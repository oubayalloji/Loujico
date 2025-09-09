namespace Loujico.Models
{
    public class InvoiceModel
    {
        public TbInvoice Invoice { get; set; }
        public List<TbFile> Files { get; set; } = new();

    }
}
