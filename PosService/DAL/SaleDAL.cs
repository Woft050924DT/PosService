using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PosService.DTO;

namespace PosService.DAL
{
    public class SalesDAL
    {
        private readonly string _conn;

        private class ProductInfo
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public int? CategoryId { get; set; }
            public int? StockQuantity { get; set; }
        }

        private class InvoiceDetailData
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal? Discount { get; set; }
            public decimal LineTotal { get; set; }
            public decimal? LinePromotionDiscount { get; set; }
        }

        public SalesDAL(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<TopSellingProductDTO>> GetTopSellingProductsAsync(DateTime? from = null, DateTime? to = null, int top = 10)
        {
            if (top <= 0)
            {
                top = 10;
            }

            const string baseSql = @"
                SELECT
                    d.ProductID,
                    p.ProductCode,
                    p.ProductName,
                    SUM(d.Quantity) AS TotalQuantity,
                    SUM(d.LineTotal) AS TotalRevenue
                FROM SalesInvoiceDetails d
                INNER JOIN SalesInvoices si ON d.InvoiceID = si.InvoiceID
                INNER JOIN Products p ON d.ProductID = p.ProductID
                WHERE si.Status = @Status";

            var innerSql = baseSql;
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Status", SqlDbType.NVarChar, 50) { Value = "Completed" }
            };

            if (from.HasValue)
            {
                innerSql += " AND si.InvoiceDate >= @FromDate";
                parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime) { Value = from.Value });
            }

            if (to.HasValue)
            {
                innerSql += " AND si.InvoiceDate <= @ToDate";
                parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime) { Value = to.Value });
            }

            innerSql += @"
                GROUP BY d.ProductID, p.ProductCode, p.ProductName";

            var sql = $@"
                SELECT TOP (@Top)
                    ProductID,
                    ProductCode,
                    ProductName,
                    TotalQuantity,
                    TotalRevenue
                FROM
                (
                    {innerSql}
                ) AS RankedProducts
                ORDER BY TotalQuantity DESC";

            var result = new List<TopSellingProductDTO>();

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddRange(parameters.ToArray());
                cmd.Parameters.Add(new SqlParameter("@Top", SqlDbType.Int) { Value = top });

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var dto = new TopSellingProductDTO
                    {
                        ProductId = reader["ProductID"] != DBNull.Value ? Convert.ToInt32(reader["ProductID"]) : 0,
                        ProductCode = reader["ProductCode"]?.ToString() ?? string.Empty,
                        ProductName = reader["ProductName"]?.ToString() ?? string.Empty,
                        TotalQuantity = reader["TotalQuantity"] != DBNull.Value ? Convert.ToInt32(reader["TotalQuantity"]) : 0,
                        TotalRevenue = reader["TotalRevenue"] != DBNull.Value ? Convert.ToDecimal(reader["TotalRevenue"]) : 0m
                    };

                    result.Add(dto);
                }
            }

            return result;
        }

        public async Task<List<SalesInvoiceDTO>> GetAllAsync(int? customerId = null, DateTime? from = null, DateTime? to = null)
        {
            const string baseSql = @"
                SELECT
                    si.InvoiceID,
                    si.InvoiceNumber,
                    si.InvoiceDate,
                    si.CustomerID,
                    c.FullName AS CustomerName,
                    si.UserID,
                    si.SubTotal,
                    si.Discount,
                    si.PromotionID,
                    p.PromotionCode,
                    p.PromotionName,
                    si.PromotionDiscount,
                    si.TotalAmount,
                    si.PaidAmount,
                    si.PaymentMethod,
                    si.Status,
                    si.Notes
                FROM SalesInvoices si
                LEFT JOIN Customers c ON si.CustomerID = c.CustomerID
                LEFT JOIN Promotions p ON si.PromotionID = p.PromotionID
                WHERE 1 = 1";

            var sql = baseSql;
            var parameters = new List<SqlParameter>();

            if (customerId.HasValue)
            {
                sql += " AND si.CustomerID = @CustomerID";
                parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = customerId.Value });
            }

            if (from.HasValue)
            {
                sql += " AND si.InvoiceDate >= @FromDate";
                parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime) { Value = from.Value });
            }

            if (to.HasValue)
            {
                sql += " AND si.InvoiceDate <= @ToDate";
                parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime) { Value = to.Value });
            }

            sql += " ORDER BY si.InvoiceDate DESC";

            var result = new List<SalesInvoiceDTO>();

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters.Count > 0)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var dto = new SalesInvoiceDTO
                    {
                        InvoiceId = reader["InvoiceID"] != DBNull.Value ? Convert.ToInt32(reader["InvoiceID"]) : 0,
                        InvoiceNumber = reader["InvoiceNumber"]?.ToString() ?? string.Empty,
                        InvoiceDate = reader["InvoiceDate"] != DBNull.Value ? Convert.ToDateTime(reader["InvoiceDate"]) : (DateTime?)null,
                        CustomerId = reader["CustomerID"] != DBNull.Value ? Convert.ToInt32(reader["CustomerID"]) : (int?)null,
                        CustomerName = reader["CustomerName"]?.ToString(),
                        UserId = reader["UserID"] != DBNull.Value ? Convert.ToInt32(reader["UserID"]) : (int?)null,
                        SubTotal = reader["SubTotal"] != DBNull.Value ? Convert.ToDecimal(reader["SubTotal"]) : (decimal?)null,
                        Discount = reader["Discount"] != DBNull.Value ? Convert.ToDecimal(reader["Discount"]) : (decimal?)null,
                        PromotionId = reader["PromotionID"] != DBNull.Value ? Convert.ToInt32(reader["PromotionID"]) : (int?)null,
                        PromotionCode = reader["PromotionCode"]?.ToString(),
                        PromotionName = reader["PromotionName"]?.ToString(),
                        PromotionDiscount = reader["PromotionDiscount"] != DBNull.Value ? Convert.ToDecimal(reader["PromotionDiscount"]) : (decimal?)null,
                        TotalAmount = reader["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalAmount"]) : (decimal?)null,
                        PaidAmount = reader["PaidAmount"] != DBNull.Value ? Convert.ToDecimal(reader["PaidAmount"]) : (decimal?)null,
                        PaymentMethod = reader["PaymentMethod"]?.ToString(),
                        Status = reader["Status"]?.ToString(),
                        Notes = reader["Notes"]?.ToString(),
                        Details = new List<SalesInvoiceDetailDTO>()
                    };

                    result.Add(dto);
                }
            }

            if (result.Count == 0)
            {
                return result;
            }

            var dict = new Dictionary<int, SalesInvoiceDTO>();
            foreach (var inv in result)
            {
                dict[inv.InvoiceId] = inv;
            }

            const string detailSql = @"
                SELECT
                    d.DetailID,
                    d.InvoiceID,
                    d.ProductID,
                    pr.ProductName,
                    d.Quantity,
                    d.UnitPrice,
                    d.Discount,
                    d.LinePromotionDiscount,
                    d.LineTotal
                FROM SalesInvoiceDetails d
                LEFT JOIN Products pr ON d.ProductID = pr.ProductID";

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(detailSql, conn))
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var invoiceId = reader["InvoiceID"] != DBNull.Value ? Convert.ToInt32(reader["InvoiceID"]) : 0;
                    if (!dict.TryGetValue(invoiceId, out var inv))
                    {
                        continue;
                    }

                    var det = new SalesInvoiceDetailDTO
                    {
                        DetailId = reader["DetailID"] != DBNull.Value ? Convert.ToInt32(reader["DetailID"]) : 0,
                        ProductId = reader["ProductID"] != DBNull.Value ? Convert.ToInt32(reader["ProductID"]) : (int?)null,
                        ProductName = reader["ProductName"]?.ToString(),
                        Quantity = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : 0,
                        UnitPrice = reader["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(reader["UnitPrice"]) : 0m,
                        Discount = reader["Discount"] != DBNull.Value ? Convert.ToDecimal(reader["Discount"]) : (decimal?)null,
                        LinePromotionDiscount = reader["LinePromotionDiscount"] != DBNull.Value ? Convert.ToDecimal(reader["LinePromotionDiscount"]) : (decimal?)null,
                        LineTotal = reader["LineTotal"] != DBNull.Value ? Convert.ToDecimal(reader["LineTotal"]) : 0m
                    };

                    inv.Details.Add(det);
                }
            }

            return result;
        }

        public async Task<List<SalesInvoiceDTO>> GetCompletedAsync(int? customerId = null, DateTime? from = null, DateTime? to = null)
        {
            const string baseSql = @"
                SELECT
                    si.InvoiceID,
                    si.InvoiceNumber,
                    si.InvoiceDate,
                    si.CustomerID,
                    c.FullName AS CustomerName,
                    si.UserID,
                    si.SubTotal,
                    si.Discount,
                    si.PromotionID,
                    p.PromotionCode,
                    p.PromotionName,
                    si.PromotionDiscount,
                    si.TotalAmount,
                    si.PaidAmount,
                    si.PaymentMethod,
                    si.Status,
                    si.Notes
                FROM SalesInvoices si
                LEFT JOIN Customers c ON si.CustomerID = c.CustomerID
                LEFT JOIN Promotions p ON si.PromotionID = p.PromotionID
                WHERE 1 = 1";

            var sql = baseSql;
            var parameters = new List<SqlParameter>();

            sql += " AND si.Status = @Status";
            parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 50) { Value = "Completed" });

            if (customerId.HasValue)
            {
                sql += " AND si.CustomerID = @CustomerID";
                parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = customerId.Value });
            }

            if (from.HasValue)
            {
                sql += " AND si.InvoiceDate >= @FromDate";
                parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime) { Value = from.Value });
            }

            if (to.HasValue)
            {
                sql += " AND si.InvoiceDate <= @ToDate";
                parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime) { Value = to.Value });
            }

            sql += " ORDER BY si.InvoiceDate DESC";

            var result = new List<SalesInvoiceDTO>();

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters.Count > 0)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var dto = new SalesInvoiceDTO
                    {
                        InvoiceId = reader["InvoiceID"] != DBNull.Value ? Convert.ToInt32(reader["InvoiceID"]) : 0,
                        InvoiceNumber = reader["InvoiceNumber"]?.ToString() ?? string.Empty,
                        InvoiceDate = reader["InvoiceDate"] != DBNull.Value ? Convert.ToDateTime(reader["InvoiceDate"]) : (DateTime?)null,
                        CustomerId = reader["CustomerID"] != DBNull.Value ? Convert.ToInt32(reader["CustomerID"]) : (int?)null,
                        CustomerName = reader["CustomerName"]?.ToString(),
                        UserId = reader["UserID"] != DBNull.Value ? Convert.ToInt32(reader["UserID"]) : (int?)null,
                        SubTotal = reader["SubTotal"] != DBNull.Value ? Convert.ToDecimal(reader["SubTotal"]) : (decimal?)null,
                        Discount = reader["Discount"] != DBNull.Value ? Convert.ToDecimal(reader["Discount"]) : (decimal?)null,
                        PromotionId = reader["PromotionID"] != DBNull.Value ? Convert.ToInt32(reader["PromotionID"]) : (int?)null,
                        PromotionCode = reader["PromotionCode"]?.ToString(),
                        PromotionName = reader["PromotionName"]?.ToString(),
                        PromotionDiscount = reader["PromotionDiscount"] != DBNull.Value ? Convert.ToDecimal(reader["PromotionDiscount"]) : (decimal?)null,
                        TotalAmount = reader["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalAmount"]) : (decimal?)null,
                        PaidAmount = reader["PaidAmount"] != DBNull.Value ? Convert.ToDecimal(reader["PaidAmount"]) : (decimal?)null,
                        PaymentMethod = reader["PaymentMethod"]?.ToString(),
                        Status = reader["Status"]?.ToString(),
                        Notes = reader["Notes"]?.ToString(),
                        Details = new List<SalesInvoiceDetailDTO>()
                    };

                    result.Add(dto);
                }
            }

            if (result.Count == 0)
            {
                return result;
            }

            var dict = new Dictionary<int, SalesInvoiceDTO>();
            foreach (var inv in result)
            {
                dict[inv.InvoiceId] = inv;
            }

            const string detailSql = @"
                SELECT
                    d.DetailID,
                    d.InvoiceID,
                    d.ProductID,
                    pr.ProductName,
                    d.Quantity,
                    d.UnitPrice,
                    d.Discount,
                    d.LinePromotionDiscount,
                    d.LineTotal
                FROM SalesInvoiceDetails d
                LEFT JOIN Products pr ON d.ProductID = pr.ProductID";

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(detailSql, conn))
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var invoiceId = reader["InvoiceID"] != DBNull.Value ? Convert.ToInt32(reader["InvoiceID"]) : 0;
                    if (!dict.TryGetValue(invoiceId, out var inv))
                    {
                        continue;
                    }

                    var det = new SalesInvoiceDetailDTO
                    {
                        DetailId = reader["DetailID"] != DBNull.Value ? Convert.ToInt32(reader["DetailID"]) : 0,
                        ProductId = reader["ProductID"] != DBNull.Value ? Convert.ToInt32(reader["ProductID"]) : (int?)null,
                        ProductName = reader["ProductName"]?.ToString(),
                        Quantity = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : 0,
                        UnitPrice = reader["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(reader["UnitPrice"]) : 0m,
                        Discount = reader["Discount"] != DBNull.Value ? Convert.ToDecimal(reader["Discount"]) : (decimal?)null,
                        LinePromotionDiscount = reader["LinePromotionDiscount"] != DBNull.Value ? Convert.ToDecimal(reader["LinePromotionDiscount"]) : (decimal?)null,
                        LineTotal = reader["LineTotal"] != DBNull.Value ? Convert.ToDecimal(reader["LineTotal"]) : 0m
                    };

                    inv.Details.Add(det);
                }
            }

            return result;
        }

        public async Task<SalesInvoiceDTO?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT
                    si.InvoiceID,
                    si.InvoiceNumber,
                    si.InvoiceDate,
                    si.CustomerID,
                    c.FullName AS CustomerName,
                    si.UserID,
                    si.SubTotal,
                    si.Discount,
                    si.PromotionID,
                    p.PromotionCode,
                    p.PromotionName,
                    si.PromotionDiscount,
                    si.TotalAmount,
                    si.PaidAmount,
                    si.PaymentMethod,
                    si.Status,
                    si.Notes
                FROM SalesInvoices si
                LEFT JOIN Customers c ON si.CustomerID = c.CustomerID
                LEFT JOIN Promotions p ON si.PromotionID = p.PromotionID
                WHERE si.InvoiceID = @InvoiceID";

            SalesInvoiceDTO? invoice = null;

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@InvoiceID", SqlDbType.Int) { Value = id });

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return null;
                }

                invoice = new SalesInvoiceDTO
                {
                    InvoiceId = reader["InvoiceID"] != DBNull.Value ? Convert.ToInt32(reader["InvoiceID"]) : 0,
                    InvoiceNumber = reader["InvoiceNumber"]?.ToString() ?? string.Empty,
                    InvoiceDate = reader["InvoiceDate"] != DBNull.Value ? Convert.ToDateTime(reader["InvoiceDate"]) : (DateTime?)null,
                    CustomerId = reader["CustomerID"] != DBNull.Value ? Convert.ToInt32(reader["CustomerID"]) : (int?)null,
                    CustomerName = reader["CustomerName"]?.ToString(),
                    UserId = reader["UserID"] != DBNull.Value ? Convert.ToInt32(reader["UserID"]) : (int?)null,
                    SubTotal = reader["SubTotal"] != DBNull.Value ? Convert.ToDecimal(reader["SubTotal"]) : (decimal?)null,
                    Discount = reader["Discount"] != DBNull.Value ? Convert.ToDecimal(reader["Discount"]) : (decimal?)null,
                    PromotionId = reader["PromotionID"] != DBNull.Value ? Convert.ToInt32(reader["PromotionID"]) : (int?)null,
                    PromotionCode = reader["PromotionCode"]?.ToString(),
                    PromotionName = reader["PromotionName"]?.ToString(),
                    PromotionDiscount = reader["PromotionDiscount"] != DBNull.Value ? Convert.ToDecimal(reader["PromotionDiscount"]) : (decimal?)null,
                    TotalAmount = reader["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalAmount"]) : (decimal?)null,
                    PaidAmount = reader["PaidAmount"] != DBNull.Value ? Convert.ToDecimal(reader["PaidAmount"]) : (decimal?)null,
                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                    Status = reader["Status"]?.ToString(),
                    Notes = reader["Notes"]?.ToString(),
                    Details = new List<SalesInvoiceDetailDTO>()
                };
            }

            const string detailSql = @"
                SELECT
                    d.DetailID,
                    d.InvoiceID,
                    d.ProductID,
                    pr.ProductName,
                    d.Quantity,
                    d.UnitPrice,
                    d.Discount,
                    d.LinePromotionDiscount,
                    d.LineTotal
                FROM SalesInvoiceDetails d
                LEFT JOIN Products pr ON d.ProductID = pr.ProductID
                WHERE d.InvoiceID = @InvoiceID";

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(detailSql, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@InvoiceID", SqlDbType.Int) { Value = id });

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var det = new SalesInvoiceDetailDTO
                    {
                        DetailId = reader["DetailID"] != DBNull.Value ? Convert.ToInt32(reader["DetailID"]) : 0,
                        ProductId = reader["ProductID"] != DBNull.Value ? Convert.ToInt32(reader["ProductID"]) : (int?)null,
                        ProductName = reader["ProductName"]?.ToString(),
                        Quantity = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : 0,
                        UnitPrice = reader["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(reader["UnitPrice"]) : 0m,
                        Discount = reader["Discount"] != DBNull.Value ? Convert.ToDecimal(reader["Discount"]) : (decimal?)null,
                        LinePromotionDiscount = reader["LinePromotionDiscount"] != DBNull.Value ? Convert.ToDecimal(reader["LinePromotionDiscount"]) : (decimal?)null,
                        LineTotal = reader["LineTotal"] != DBNull.Value ? Convert.ToDecimal(reader["LineTotal"]) : 0m
                    };

                    invoice!.Details.Add(det);
                }
            }

            return invoice;
        }

        public async Task<SalesInvoiceDTO> CreateAsync(CreateSalesInvoiceDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Details == null || !dto.Details.Any()) throw new InvalidOperationException("Invoice must contain at least one detail.");

            var productIds = dto.Details.Select(d => d.ProductId).Distinct().ToList();
            if (productIds.Count == 0) throw new InvalidOperationException("Invoice must contain at least one product.");

            var products = new Dictionary<int, ProductInfo>();

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;

                var placeholders = new List<string>();
                for (int i = 0; i < productIds.Count; i++)
                {
                    var paramName = "@p" + i;
                    placeholders.Add(paramName);
                    cmd.Parameters.Add(new SqlParameter(paramName, SqlDbType.Int) { Value = productIds[i] });
                }

                cmd.CommandText = $@"
                    SELECT 
                        ProductID,
                        ProductName,
                        CategoryID,
                        StockQuantity
                    FROM Products
                    WHERE ProductID IN ({string.Join(",", placeholders)})";

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var info = new ProductInfo
                    {
                        ProductId = reader["ProductID"] != DBNull.Value ? Convert.ToInt32(reader["ProductID"]) : 0,
                        ProductName = reader["ProductName"]?.ToString() ?? string.Empty,
                        CategoryId = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : (int?)null,
                        StockQuantity = reader["StockQuantity"] != DBNull.Value ? Convert.ToInt32(reader["StockQuantity"]) : (int?)null
                    };

                    products[info.ProductId] = info;
                }
            }

            foreach (var d in dto.Details)
            {
                if (!products.ContainsKey(d.ProductId))
                {
                    throw new InvalidOperationException($"ProductId {d.ProductId} not found.");
                }
            }

            decimal subTotal = 0m;
            var detailEntities = new List<InvoiceDetailData>();

            foreach (var d in dto.Details)
            {
                var prod = products[d.ProductId];
                if (prod.StockQuantity.HasValue && prod.StockQuantity.Value < d.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product '{prod.ProductName}' (id={prod.ProductId}). Available: {prod.StockQuantity}, requested: {d.Quantity}");
                }

                decimal lineDiscount = d.Discount ?? 0m;
                decimal lineTotal = (d.UnitPrice * d.Quantity) - lineDiscount;
                if (lineTotal < 0) lineTotal = 0m;

                subTotal += lineTotal;

                var det = new InvoiceDetailData
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Discount = d.Discount,
                    LineTotal = lineTotal,
                    LinePromotionDiscount = null
                };

                detailEntities.Add(det);
            }

            decimal overallDiscount = dto.Discount ?? 0m;
            int? promotionId = null;
            decimal promotionDiscount = 0m;

            if (dto.PromotionId.HasValue || !string.IsNullOrWhiteSpace(dto.PromotionCode))
            {
                using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                var promoSql = @"
                    SELECT
                        PromotionID,
                        DiscountType,
                        DiscountValue,
                        MinOrderAmount,
                        ApplyTo,
                        IsActive,
                        StartDate,
                        EndDate
                    FROM Promotions
                    WHERE 1 = 1";

                var promoCmd = new SqlCommand();
                promoCmd.Connection = conn;

                if (dto.PromotionId.HasValue)
                {
                    promoSql += " AND PromotionID = @PromotionID";
                    promoCmd.Parameters.Add(new SqlParameter("@PromotionID", SqlDbType.Int) { Value = dto.PromotionId.Value });
                }
                if (!string.IsNullOrWhiteSpace(dto.PromotionCode))
                {
                    promoSql += " AND PromotionCode = @PromotionCode";
                    promoCmd.Parameters.Add(new SqlParameter("@PromotionCode", SqlDbType.NVarChar, 50) { Value = dto.PromotionCode!.Trim() });
                }

                promoCmd.CommandText = promoSql;

                string discountType;
                decimal discountValue;
                decimal? minOrderAmount;
                string applyTo;
                bool isActive;
                DateTime startDate;
                DateTime? endDate;

                using (var reader = await promoCmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        throw new InvalidOperationException("Promotion not found.");
                    }

                    promotionId = reader["PromotionID"] != DBNull.Value ? Convert.ToInt32(reader["PromotionID"]) : (int?)null;
                    discountType = reader["DiscountType"]?.ToString() ?? string.Empty;
                    discountValue = reader["DiscountValue"] != DBNull.Value ? Convert.ToDecimal(reader["DiscountValue"]) : 0m;
                    minOrderAmount = reader["MinOrderAmount"] != DBNull.Value ? Convert.ToDecimal(reader["MinOrderAmount"]) : (decimal?)null;
                    applyTo = reader["ApplyTo"]?.ToString() ?? string.Empty;
                    isActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]);
                    startDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]) : DateTime.MinValue;
                    endDate = reader["EndDate"] != DBNull.Value ? Convert.ToDateTime(reader["EndDate"]) : (DateTime?)null;
                }

                var today = DateTime.Today;
                if (!isActive || startDate.Date > today || (endDate.HasValue && endDate.Value.Date < today))
                {
                    throw new InvalidOperationException("Promotion is not active.");
                }

                if (minOrderAmount.HasValue && subTotal < minOrderAmount.Value)
                {
                    throw new InvalidOperationException("Order does not meet minimum amount for promotion.");
                }

                var discountTypeLower = discountType.ToLowerInvariant();
                var applyToLower = applyTo.ToLowerInvariant();

                if (applyToLower == "order" || applyToLower == "invoice")
                {
                    var baseAmount = subTotal;
                    decimal promoAmount;
                    if (discountTypeLower == "percent" || discountTypeLower == "percentage")
                    {
                        promoAmount = baseAmount * discountValue / 100m;
                    }
                    else
                    {
                        promoAmount = discountValue;
                    }

                    if (promoAmount < 0m) promoAmount = 0m;
                    if (promoAmount > baseAmount) promoAmount = baseAmount;
                    promotionDiscount = promoAmount;
                }
                else if (applyToLower == "product" || applyToLower == "category")
                {
                    var promotionProductIds = new HashSet<int>();
                    var promotionCategoryIds = new HashSet<int>();

                    const string prodSql = @"
                        SELECT ProductID
                        FROM PromotionProducts
                        WHERE PromotionID = @PromotionID";

                    using (var prodCmd = new SqlCommand(prodSql, conn))
                    {
                        prodCmd.Parameters.Add(new SqlParameter("@PromotionID", SqlDbType.Int) { Value = promotionId!.Value });
                        using var prodReader = await prodCmd.ExecuteReaderAsync();
                        while (await prodReader.ReadAsync())
                        {
                            var pid = prodReader["ProductID"] != DBNull.Value ? Convert.ToInt32(prodReader["ProductID"]) : 0;
                            promotionProductIds.Add(pid);
                        }
                    }

                    const string catSql = @"
                        SELECT CategoryID
                        FROM PromotionCategories
                        WHERE PromotionID = @PromotionID";

                    using (var catCmd = new SqlCommand(catSql, conn))
                    {
                        catCmd.Parameters.Add(new SqlParameter("@PromotionID", SqlDbType.Int) { Value = promotionId!.Value });
                        using var catReader = await catCmd.ExecuteReaderAsync();
                        while (await catReader.ReadAsync())
                        {
                            var cid = catReader["CategoryID"] != DBNull.Value ? Convert.ToInt32(catReader["CategoryID"]) : 0;
                            promotionCategoryIds.Add(cid);
                        }
                    }

                    foreach (var det in detailEntities)
                    {
                        var prod = products[det.ProductId];

                        bool match = false;
                        if (applyToLower == "product" && promotionProductIds.Contains(prod.ProductId))
                        {
                            match = true;
                        }
                        else if (applyToLower == "category" && prod.CategoryId.HasValue && promotionCategoryIds.Contains(prod.CategoryId.Value))
                        {
                            match = true;
                        }

                        if (!match)
                        {
                            continue;
                        }

                        var lineBaseAmount = det.LineTotal;
                        decimal linePromo;
                        if (discountTypeLower == "percent" || discountTypeLower == "percentage")
                        {
                            linePromo = lineBaseAmount * discountValue / 100m;
                        }
                        else
                        {
                            linePromo = discountValue;
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

            string invoiceNumber = !string.IsNullOrWhiteSpace(dto.InvoiceNumber)
                ? dto.InvoiceNumber!
                : $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}";

            int invoiceId;

            using (var conn = new SqlConnection(_conn))
            {
                await conn.OpenAsync();
                using var tx = await conn.BeginTransactionAsync();

                const string insertInvoiceSql = @"
                    INSERT INTO SalesInvoices
                    (
                        InvoiceNumber,
                        InvoiceDate,
                        CustomerID,
                        UserID,
                        SubTotal,
                        Discount,
                        PromotionID,
                        PromotionDiscount,
                        TotalAmount,
                        PaidAmount,
                        PaymentMethod,
                        Status,
                        Notes
                    )
                    OUTPUT INSERTED.InvoiceID
                    VALUES
                    (
                        @InvoiceNumber,
                        @InvoiceDate,
                        @CustomerID,
                        @UserID,
                        @SubTotal,
                        @Discount,
                        @PromotionID,
                        @PromotionDiscount,
                        @TotalAmount,
                        @PaidAmount,
                        @PaymentMethod,
                        @Status,
                        @Notes
                    )";

                using (var cmd = new SqlCommand(insertInvoiceSql, conn, (SqlTransaction)tx))
                {
                    cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);
                    cmd.Parameters.AddWithValue("@InvoiceDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CustomerID", dto.CustomerId.HasValue ? (object)dto.CustomerId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserID", dto.UserId.HasValue ? (object)dto.UserId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@SubTotal", subTotal);
                    cmd.Parameters.AddWithValue("@Discount", overallDiscount);
                    cmd.Parameters.AddWithValue("@PromotionID", promotionId.HasValue ? (object)promotionId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@PromotionDiscount", promotionDiscount);
                    cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                    cmd.Parameters.AddWithValue("@PaidAmount", dto.PaidAmount ?? 0m);
                    cmd.Parameters.AddWithValue("@PaymentMethod", (object?)dto.PaymentMethod ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", "Completed");
                    cmd.Parameters.AddWithValue("@Notes", (object?)dto.Notes ?? DBNull.Value);

                    using var reader = await cmd.ExecuteReaderAsync();
                    if (!await reader.ReadAsync())
                    {
                        await tx.RollbackAsync();
                        throw new InvalidOperationException("Failed to create invoice.");
                    }

                    invoiceId = reader["InvoiceID"] != DBNull.Value ? Convert.ToInt32(reader["InvoiceID"]) : 0;
                }

                const string insertDetailSql = @"
                    INSERT INTO SalesInvoiceDetails
                    (
                        InvoiceID,
                        ProductID,
                        Quantity,
                        UnitPrice,
                        Discount,
                        LineTotal,
                        LinePromotionDiscount
                    )
                    VALUES
                    (
                        @InvoiceID,
                        @ProductID,
                        @Quantity,
                        @UnitPrice,
                        @Discount,
                        @LineTotal,
                        @LinePromotionDiscount
                    )";

                using (var detailCmd = new SqlCommand(insertDetailSql, conn, (SqlTransaction)tx))
                {
                    detailCmd.Parameters.Add("@InvoiceID", SqlDbType.Int);
                    detailCmd.Parameters.Add("@ProductID", SqlDbType.Int);
                    detailCmd.Parameters.Add("@Quantity", SqlDbType.Int);
                    detailCmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Precision = 18;
                    detailCmd.Parameters["@UnitPrice"].Scale = 2;
                    detailCmd.Parameters.Add("@Discount", SqlDbType.Decimal).Precision = 18;
                    detailCmd.Parameters["@Discount"].Scale = 2;
                    detailCmd.Parameters.Add("@LineTotal", SqlDbType.Decimal).Precision = 18;
                    detailCmd.Parameters["@LineTotal"].Scale = 2;
                    detailCmd.Parameters.Add("@LinePromotionDiscount", SqlDbType.Decimal).Precision = 18;
                    detailCmd.Parameters["@LinePromotionDiscount"].Scale = 2;

                    foreach (var det in detailEntities)
                    {
                        detailCmd.Parameters["@InvoiceID"].Value = invoiceId;
                        detailCmd.Parameters["@ProductID"].Value = det.ProductId;
                        detailCmd.Parameters["@Quantity"].Value = det.Quantity;
                        detailCmd.Parameters["@UnitPrice"].Value = det.UnitPrice;
                        detailCmd.Parameters["@Discount"].Value = det.Discount ?? 0m;
                        detailCmd.Parameters["@LineTotal"].Value = det.LineTotal;
                        detailCmd.Parameters["@LinePromotionDiscount"].Value = det.LinePromotionDiscount ?? 0m;

                        await detailCmd.ExecuteNonQueryAsync();
                    }
                }

                const string updateStockSql = @"
                    UPDATE Products
                    SET StockQuantity = ISNULL(StockQuantity, 0) - @Quantity
                    WHERE ProductID = @ProductID";

                using (var stockCmd = new SqlCommand(updateStockSql, conn, (SqlTransaction)tx))
                {
                    stockCmd.Parameters.Add("@ProductID", SqlDbType.Int);
                    stockCmd.Parameters.Add("@Quantity", SqlDbType.Int);

                    foreach (var det in detailEntities)
                    {
                        stockCmd.Parameters["@ProductID"].Value = det.ProductId;
                        stockCmd.Parameters["@Quantity"].Value = det.Quantity;
                        await stockCmd.ExecuteNonQueryAsync();
                    }
                }

                await tx.CommitAsync();
            }

            var created = await GetByIdAsync(invoiceId);
            if (created == null) throw new InvalidOperationException("Failed to load created invoice.");
            return created;
        }
    }
}
