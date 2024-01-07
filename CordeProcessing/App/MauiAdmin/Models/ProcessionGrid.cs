using MauiAdmin.ViewModels;

namespace MauiAdmin.Models;

public class ProcessionGrid
{
    public string ProcessionId { get; set; }
    private List<ProcessionViewModel> ProcessionViewModels { get; set; }
}