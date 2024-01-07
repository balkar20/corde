using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiAdmin.ViewModels;

public class ProcessionViewModel
{
    private string _processingItemId;
    private ProcessorTypes _processorType;
    private string _processorThreadId;
    private string _processorResult;

    // private string title;
    // private string company;
    // private int price;
    public string ProcessingItemId
    {
        get => _processingItemId;
        set
        {
            _processingItemId = value;
            OnPropertyChanged("ProcessingItemId");
        }
    }

    public ProcessorTypes ProcessorType
    {
        get => _processorType;
        set
        {
            _processorType = value;
            OnPropertyChanged("ProcessorType");
        }
    }

    public string ProcessorThreadId
    {
        get => _processorThreadId;
        set
        {
            _processorThreadId = value;
            OnPropertyChanged("ProcessorThreadId");
        }
    }

    public string ProcessorResult
    {
        get => _processorResult;
        set
        {
            _processorResult = value;
            OnPropertyChanged("ProcessorResult");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName]string prop = "")
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
    }
}

public enum ProcessorTypes
{
    Type,
    Color,
    Mark,
    Danger,
    Season,
    Traffic
}