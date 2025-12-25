using Microsoft.Extensions.Options;
using DTO;
using System.Data;
using DAL.Models;

public class DAL_HoaDon
{
    private readonly DefaultConnect db;
    private readonly DTO_I_DBHelper dbHelped;
    public DAL_HoaDon(IOptions<DefaultConnect> options, DTO_I_DBHelper _dbHelped)
    {
        db = options.Value;
        dbHelped= _dbHelped;
    }

    public bool create(PurchaseOrder model,int SupplierId, int UserId)
    {
        string msgError = "";
        try
        {
            var result = dbHelped.ExecuteScalarSProcedureWithTransaction(out msgError, "createHoaDon",
            "@PurchaseID", model.PurchaseId,
            "@PurchaseNumber", model.PurchaseNumber,
            "@PurchaseDate", model.PurchaseDate,
            "@SuppilerID", SupplierId,
            "@UserId", UserId,
            "@TotalAmount", model.TotalAmount,
            "@PaidAmount", model.PaidAmount,
            "@Status", model.Status,
            "@Notes", model.Notes);

            if ((result != null && !string.IsNullOrEmpty(result.ToString())) || !string.IsNullOrEmpty(msgError))
            {
                throw new Exception(Convert.ToString(result) + msgError);
            }
            return true;
}
        catch (Exception ex)
        {
            throw ex;
        }
    }
}