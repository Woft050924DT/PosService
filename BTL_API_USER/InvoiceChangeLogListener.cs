using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DTO_Models;
using HandleLogChangeDB;

namespace DAL
{
        public class InvoiceChangeLogListener : BackgroundService
        {
            private readonly IServiceScopeFactory _scopeFactory;
            private readonly ILogger<InvoiceChangeLogListener> _logger;
            private readonly HandleLog handleLog;

            public InvoiceChangeLogListener(
            IServiceScopeFactory scopeFactory,
            ILogger<InvoiceChangeLogListener> logger,
            HandleLog _handleLog
            )
            {
                _scopeFactory = scopeFactory;
                _logger = logger;
                handleLog = _handleLog;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                _logger.LogInformation("InvoiceChangeLog listener started");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<HdvContext>();

                    var logs = await db.invoice_change_log
                            .Where(x => !x.processed)
                            .OrderBy(x => x.id)
                            .Take(50)
                            .ToListAsync(stoppingToken);

                        if (logs.Count == 0)
                        {
                            await Task.Delay(2000, stoppingToken);
                            continue;
                        }

                        foreach (var log in logs)
                        {
                        handleLog.handleLog(log);
                            log.processed = true;
                        }

                        await db.SaveChangesAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing invoice_change_log");
                        await Task.Delay(5000, stoppingToken);
                    }
                }
            }
        }
}
