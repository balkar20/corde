using CommunityToolkit.Maui.Markup;
using MauiAdmin.Models;
using MauiAdmin.ViewModels;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiAdmin.Pages;

public class GridContentView: CollectionView
{
    public GridContentView()
    {
        // Content = new Label()
        // {
        //     Text = "Voko"
        // };
        BindingContext = new ProcessionContextGrid();
        var randomProcessionViewModels = new List<ProcessionViewModel>
        {
            new ProcessionViewModel
            {
                ProcessingItemId = "12345",
                ProcessorType = ProcessorTypes.Color,
                ProcessorThreadId = "Thread1",
                ProcessorResult = "Red"
            },
            new ProcessionViewModel
            {
                ProcessingItemId = "67890",
                ProcessorType = ProcessorTypes.Mark,
                ProcessorThreadId = "Thread2",
                ProcessorResult = "Approved"
            },
            new ProcessionViewModel
            {
                ProcessingItemId = "54321",
                ProcessorType = ProcessorTypes.Type,
                ProcessorThreadId = "Thread3",
                ProcessorResult = "Fruit"
            },
            // Add more random instances as needed...
        };
        ItemsSource = randomProcessionViewModels;
        // ItemTemplate = new DataTemplate(() =>
        // {
        //     new StackLayout()
        //     {
        //
        //     };
        // });
        // Content = new Grid
        // {
        //     RowDefinitions = Rows.Define(
        //         (Row.Username, 30),
        //         (Row.Password, 30),
        //         (Row.Submit, Star)),
        //
        //     ColumnDefinitions = Columns.Define(
        //         (Column.Description, Star),
        //         (Column.UserInput, Star)),
        //
        //     Children =
        //     {
        //         
        //         new CollectionView ()
        //         {
        //             ItemsSource = randomProcessionViewModels
        //         },
        //         // new Label()
        //         //     .Text("Username")
        //         //     .Row(Row.Username).Column(Column.Description),
        //         //
        //         // new Entry()
        //         //     .Placeholder("Username")
        //         //     .Row(Row.Username).Column(Column.UserInput),
        //         //
        //         // new Label()
        //         //     .Text("Password")
        //         //     .Row(Row.Password).Column(Column.Description),
        //         //
        //         // new Entry { IsPassword = true }
        //         //     .Placeholder("Password")
        //         //     .Row(Row.Password).Column(Column.UserInput),
        //         //
        //         // new Button()
        //         //     .Text("Submit")
        //         //     .Row(Row.Password).RowSpan(All<Column>())
        //     }
        // };

    }
    
    enum Row { Username, Password, Submit }

    enum Column { Description, UserInput }
}