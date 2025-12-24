using Microsoft.Extensions.Options;

public class dal_hoaDon : ControllerBase
{
    private readonly defaulseConnect _db;

    public dal_hoaDon(IOptions<defaulseConnect> options)
    {
        _db = options.Value;
    }

    // Sử dụng: _db.ConnectionString
}