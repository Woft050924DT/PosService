
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PosService.DTO;
using PosService.Models;

namespace PosService.DAL
{
    public class SalesDAL
    {
        private readonly HDVContext _db;

        public SalesDAL(HDVContext db)
        {
            _db = db;
        }

        public async Task<List<SalesInvoiceDTO>> GetAllAsync(int? customerId = null, DateTime? from = null, DateTime? to = null)
        {
            var query = _db.SalesInvoices
                .AsNoTracking()
                .Include(si => si.Customer)
                .Include(si => si.User)
                .Include(si => si.Promotion)
                .Include(si => si.SalesInvoiceDetails).ThenInclude(d => d.Product)
                .AsQueryable();

            if (customerId.HasValue)
                query = query.Where(x => x.CustomerId == customerId.Value);

            if (from.HasValue)
                query = query.Where(x => x.InvoiceDate >= from.Value);

            if (to.HasValue)
                query = query.Where(x => x.InvoiceDate <= to.Value);

            var list = await query
                .OrderByDescending(x => x.InvoiceDate)
                .Select(si => new SalesInvoiceDTO
                {
                    InvoiceId = si.InvoiceId,
                    InvoiceNumber = si.InvoiceNumber,
                    InvoiceDate = si.InvoiceDate,
                    CustomerId = si.CustomerId,
                    CustomerName = si.Customer != null ? si.Customer.FullName : null,
                    UserId = si.UserId,
                    SubTotal = si.SubTotal,
                    Discount = si.Discount,
                    PromotionId = si.PromotionId,
                    PromotionCode = si.Promotion != null ? si.Promotion.PromotionCode : null,
                    PromotionName = si.Promotion != null ? si.Promotion.PromotionName : null,
                    PromotionDiscount = si.PromotionDiscount,
                    TotalAmount = si.TotalAmount,
                    PaidAmount = si.PaidAmount,
                    PaymentMethod = si.PaymentMethod,
                    Status = si.Status,
                    Notes = si.Notes,
                    Details = si.SalesInvoiceDetails.Select(d => new SalesInvoiceDetailDTO
                    {
                        DetailId = d.DetailId,
                        ProductId = d.ProductId,
                        ProductName = d.Product != null ? d.Product.ProductName : null,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Discount = d.Discount,
                        LinePromotionDiscount = d.LinePromotionDiscount,
                        LineTotal = d.LineTotal
                    }).ToList()
                })
                .ToListAsync();

            return list;
        }

        public async Task<SalesInvoiceDTO?> GetByIdAsync(int id)
        {
            var si = await _db.SalesInvoices
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.User)
                .Include(x => x.Promotion)
                .Include(x => x.SalesInvoiceDetails).ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(x => x.InvoiceId == id);

            if (si == null) return null;

            return new SalesInvoiceDTO
            {
                InvoiceId = si.InvoiceId,
                InvoiceNumber = si.InvoiceNumber,
                InvoiceDate = si.InvoiceDate,
                CustomerId = si.CustomerId,
                CustomerName = si.Customer?.FullName,
                UserId = si.UserId,
                SubTotal = si.SubTotal,
                Discount = si.Discount,
                PromotionId = si.PromotionId,
                PromotionCode = si.Promotion?.PromotionCode,
                PromotionName = si.Promotion?.PromotionName,
                PromotionDiscount = si.PromotionDiscount,
                TotalAmount = si.TotalAmount,
                PaidAmount = si.PaidAmount,
                PaymentMethod = si.PaymentMethod,
                Status = si.Status,
                Notes = si.Notes,
                Details = si.SalesInvoiceDetails.Select(d => new SalesInvoiceDetailDTO
                {
                    DetailId = d.DetailId,
                    ProductId = d.ProductId,
                    ProductName = d.Product?.ProductName,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Discount = d.Discount,
                    LinePromotionDiscount = d.LinePromotionDiscount,
                    LineTotal = d.LineTotal
                }).ToList()
            };
        }

        public async Task<SalesInvoiceDTO> CreateAsync(CreateSalesInvoiceDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Details == null || !dto.Details.Any()) throw new InvalidOperationException("Invoice must contain at least one detail.");

            // Load products referenced in details
            var productIds = dto.Details.Select(d => d.ProductId).Distinct().ToList();
            var products = await _db.Products.Where(p => productIds.Contains(p.ProductId)).ToListAsync();

            // Validate all products exist
            foreach (var d in dto.Details)
            {
                if (!products.Any(p => p.ProductId == d.ProductId))
                    throw new InvalidOperationException($"ProductId {d.ProductId} not found.");
            }

            decimal subTotal = 0m;
            var detailEntities = new List<SalesInvoiceDetail>();
            foreach (var d in dto.Details)
            {
                var prod = products.First(p => p.ProductId == d.ProductId);
                if (prod.StockQuantity.HasValue && prod.StockQuantity.Value < d.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product '{prod.ProductName}' (id={prod.ProductId}). Available: {prod.StockQuantity}, requested: {d.Quantity}");
                }

                decimal lineDiscount = d.Discount ?? 0m;
                decimal lineTotal = (d.UnitPrice * d.Quantity) - lineDiscount;
                if (lineTotal < 0) lineTotal = 0m;

                subTotal += lineTotal;

                var detailEntity = new SalesInvoiceDetail
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Discount = d.Discount,
                    LineTotal = lineTotal
                };

                detailEntities.Add(detailEntity);
            }

            decimal overallDiscount = dto.Discount ?? 0m;
            Promotion? promotion = null;
            decimal promotionDiscount = 0m;

            if (dto.PromotionId.HasValue || !string.IsNullOrWhiteSpace(dto.PromotionCode))
            {
                var query = _db.Promotions
                    .Include(p => p.Products)
                    .Include(p => p.Categories)
                    .AsQueryable();

                if (dto.PromotionId.HasValue)
                {
                    query = query.Where(p => p.PromotionId == dto.PromotionId.Value);
                }
                else if (!string.IsNullOrWhiteSpace(dto.PromotionCode)) 
                {
                    var code = dto.PromotionCode!.Trim();
                    query = query.Where(p => p.PromotionCode == code);
                }

                promotion = await query.FirstOrDefaultAsync();
                if (promotion == null)
                {
                    throw new InvalidOperationException("Promotion not found.");
                }

                var today = DateOnly.FromDateTime(DateTime.Today);
                if (promotion.IsActive != true || promotion.StartDate > today || (promotion.EndDate.HasValue && promotion.EndDate.Value < today))
                {
                    throw new InvalidOperationException("Promotion is not active.");
                }

                if (promotion.MinOrderAmount.HasValue && subTotal < promotion.MinOrderAmount.Value)
                {
                    throw new InvalidOperationException("Order does not meet minimum amount for promotion.");
                }

                var discountType = promotion.DiscountType.ToLowerInvariant();
                var applyTo = promotion.ApplyTo.ToLowerInvariant();

                if (applyTo == "order" || applyTo == "invoice")
                {
                    var baseAmount = subTotal;
                    decimal promoAmount;
                    if (discountType == "percent" || discountType == "percentage")
                    {
                        promoAmount = baseAmount * promotion.DiscountValue / 100m;
                    }
                    else
                    {
                        promoAmount = promotion.DiscountValue;
                    }

                    if (promoAmount < 0m) promoAmount = 0m;
                    if (promoAmount > baseAmount) promoAmount = baseAmount;
                    promotionDiscount = promoAmount;
                }
                else if (applyTo == "product" || applyTo == "category")
                {
                    var promotionProductIds = new HashSet<int>(promotion.Products.Select(p => p.ProductId));
                    var promotionCategoryIds = new HashSet<int>(promotion.Categories.Select(c => c.CategoryId));

                    foreach (var det in detailEntities)
                    {
                        if (!det.ProductId.HasValue)
                        {
                            continue;
                        }

                        var prod = products.First(p => p.ProductId == det.ProductId.Value);

                        bool match = false;
                        if (applyTo == "product" && promotionProductIds.Contains(prod.ProductId))
                        {
                            match = true;
                        }
                        else if (applyTo == "category" && prod.CategoryId.HasValue && promotionCategoryIds.Contains(prod.CategoryId.Value))
                        {
                            match = true;
                        }

                        if (!match)
                        {
                            continue;
                        }

                        var lineBaseAmount = det.LineTotal;
                        decimal linePromo;
                        if (discountType == "percent" || discountType == "percentage")
                        {
                            linePromo = lineBaseAmount * promotion.DiscountValue / 100m;
                        }
                        else
                        {
                            linePromo = promotion.DiscountValue;
                        }

                        if (linePromo < 0m) linePromo = 0m;
                        if (linePromo > lineBaseAmount) linePromo = lineBaseAmount;

                        det.LinePromotionDiscount = linePromo;
                        promotionDiscount += linePromo;
                    }
                }
            }

            decimal totalAmount = subTotal - overallDiscount - promotionDiscount;
            if (totalAmount < 0m) totalAmount = 0m;

            // Generate invoice number if not provided
            string invoiceNumber = !string.IsNullOrWhiteSpace(dto.InvoiceNumber)
                ? dto.InvoiceNumber!
                : $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}";

            var invoiceEntity = new SalesInvoice
            {
                InvoiceNumber = invoiceNumber,
                InvoiceDate = DateTime.Now,
                CustomerId = dto.CustomerId,
                UserId = dto.UserId,
                SubTotal = subTotal,
                Discount = overallDiscount,
                PromotionId = promotion?.PromotionId,
                PromotionDiscount = promotionDiscount,
                TotalAmount = totalAmount,
                PaidAmount = dto.PaidAmount,
                PaymentMethod = dto.PaymentMethod,
                Status = "Completed",
                Notes = dto.Notes
            };

            // Transactional save: create invoice, details, update product stock
            using var txn = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.SalesInvoices.Add(invoiceEntity);
                await _db.SaveChangesAsync(); // get InvoiceId

                // attach details with InvoiceId
                foreach (var det in detailEntities)
                {
                    det.InvoiceId = invoiceEntity.InvoiceId;
                    _db.SalesInvoiceDetails.Add(det);

                    // update product stock (if present)
                    var prod = products.First(p => p.ProductId == det.ProductId);
                    if (prod.StockQuantity.HasValue)
                        prod.StockQuantity = prod.StockQuantity.Value - det.Quantity;
                    // else do nothing (stock not tracked)
                }

                await _db.SaveChangesAsync();
                await txn.CommitAsync();
            }
            catch
            {
                await txn.RollbackAsync();
                throw;
            }

            // Return created invoice DTO (reload to include names)
            var created = await GetByIdAsync(invoiceEntity.InvoiceId);
            if (created == null) throw new InvalidOperationException("Failed to load created invoice.");
            return created;
        }
    }
}
