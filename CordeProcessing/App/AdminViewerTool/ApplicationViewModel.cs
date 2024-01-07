using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AdminViewerTool;

public class ApplicationViewModel
{
    private ProcessionViewModel selectedProcessionViewModel;
 
    public ObservableCollection<ProcessionViewModel> ProcessionViewModels { get; set; }
    public ProcessionViewModel SelectedProcessionViewModel
    {
        get { return selectedProcessionViewModel; }
        set
        {
            selectedProcessionViewModel = value;
            OnPropertyChanged("SelectedProcessionViewModel");
        }
    }
 
    public ApplicationViewModel()
    {
        ProcessionViewModels = new ObservableCollection<ProcessionViewModel>
        {
            new ProcessionViewModel { ProcessingItemId= "iProcessionViewModel 7", ProcessorResult= "Apple", ProcessorThreadId= "56000", ProcessorType = ProcessorTypes.Type},
            new ProcessionViewModel {ProcessingItemId="Galaxy S7 Edge", ProcessorResult="Samsung", ProcessorThreadId ="60000", ProcessorType = ProcessorTypes.Color },
            new ProcessionViewModel {ProcessingItemId="Elite x3", ProcessorResult="HP", ProcessorThreadId="56000", ProcessorType = ProcessorTypes.Mark },
            new ProcessionViewModel {ProcessingItemId="Mi5S", ProcessorResult="Xiaomi", ProcessorThreadId="35000", ProcessorType = ProcessorTypes.Traffic }
        };
    }
 
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName]string prop = "")
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
    }
}