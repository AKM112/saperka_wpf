using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace saperka;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<Field> fields = new List<Field>();
    // public static List<Button> 
    public MainWindow()
    {
        InitializeComponent();
        newGame(this);
    }

    public static void newGame(MainWindow instance)
    {
        int xField = 20;
        int yField = 20;
        Field.width = xField;
        Field.height = yField;
        instance.SaperField.Columns = xField;
        instance.SaperField.Rows = yField;
        instance.SaperField.Children.Clear();
        Field.Fields.Clear();
        for (int i = 0; i < xField; i++)
            for (int j = 0; j < yField; j++)
            {
                Field field = new Field();
                Field.Fields.Add(field);
                field.Content = "";
                instance.SaperField.Children.Add(field);
            }
        Field.Setup();
    }
}