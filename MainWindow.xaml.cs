using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShippingHub
{
    public partial class MainWindow : Window
    {
        public class Package
        {
            public static List<Package> list = new List<Package>();
            public string arrival { get; set; } // Date and time the package arrived.
            public int pID { get; set; }        // Package ID.
            public string addr { get; set; }    // Address.
            public string city { get; set; }
            public string state { get; set; }
            public int zip { get; set; }

            private static int list_size = 0;   // The number of packages in the list.
            private static int list_index = 0;  // The current list index.

            public Package() { }
            public Package(string arrival, int pID, string addr, string city, string state, int zip)
            {
                this.arrival = arrival;
                this.pID = pID;
                this.addr = addr;
                this.city = city;
                this.state = state;
                this.zip = zip;
            }

            public int Get_Size() { return list_size; }
            public void Set_Size(bool increasing) {
                if (increasing) list_size += 1;
                else list_size -= 1;
            }

            public int Get_Index() { return list_index; }
            public void Set_Index(bool increasing)
            {
                if (increasing) list_index += 1;
                else list_index -= 1;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        // Generate package id number for each package to arrive at shipping hub.
        private void Button_Scan_New(object sender, RoutedEventArgs e)
        {
            DateTime time = DateTime.Now;
            Random rand = new Random();

            // Display current date and time in the "Arrived at:" textbox.
            // Display and generate a package ID.
            arrival.Text = time.ToString("F");
            pID.Text = rand.Next(10000000, 90000000).ToString();

            addr.IsEnabled = true;
            btn_add.IsEnabled = true;
        }

        // Add package to the list.
        private void Button_Add(object sender, RoutedEventArgs e)
        {
            Package package = new Package();
            package = Create_Package(package);

            // Check if all fields are filled.
            if (package != null)
            {
                // Add package and increment list_size, list_index.
                Package.list.Add(package);
                package.Set_Size(true);
                package.Set_Index(true);
            }

            else 
                return;

            // Enable buttons for navigation.
            if (package.Get_Size() > 0)
                btn_back.IsEnabled = true;
            else
                btn_back.IsEnabled = false;

            // Enable next button.
            if (package.Get_Size() > package.Get_Index())
                btn_next.IsEnabled = true;
            else
                btn_next.IsEnabled = false;

            // Enable remove, edit buttons.
            if (package.Get_Size() > 0)
            {
                btn_remove.IsEnabled = true;
                btn_edit.IsEnabled = true;
            }

            else
            {
                btn_back.IsEnabled = false;
                btn_next.IsEnabled = false;
                btn_remove.IsEnabled = false;
                btn_edit.IsEnabled = false;
            }

            Clear_Fields();
            addr.IsEnabled = false;
            btn_add.IsEnabled = false;
        }

        // Update current package package information.
        private void Button_Edit(object sender, RoutedEventArgs e)
        {
            Package package = new Package();
            package = Create_Package(package);

            // Replace the current package with the updated input.
            try
            {
                if (package != null)
                {
                    // Package replace = Package.list.Find(p => package.pID.ToString() == pID.Text);
                    Package.list[package.Get_Index()] = package; //// MAKE SURE THIS IS IN BOUNDS (DELETE COMMENT)
                }
            }

            catch
            {
                Console.WriteLine("***Index out of range (Button_Edit).");
            }

            Clear_Fields();
        }

        // Remove package from the list.
        private void Button_Remove(object sender, RoutedEventArgs e)
        {
            Package package = new Package();
            package = Create_Package(package);

            try
            {
                if (package != null)
                {
                    Package.list.Remove(package);
                    package.Set_Size(false);
                }
            }

            catch
            {
                Console.WriteLine("***(Button_Remove).");
            }

            Clear_Fields();
        }

        // Navigate to the next item in the list.
        private void Button_Next(object sender, RoutedEventArgs e)
        {
            Package obj = new Package();
            Package current = new Package();

            try
            {
                current = Package.list[obj.Get_Index() + 1];
            }

            catch
            {
                Console.WriteLine("***Index is out of range for List of packages (Button_Next).");
            }

            finally
            {
                btn_back.IsEnabled = true;
            }

            // Disable next button when reaching the end of the list.
            if (obj.Get_Size() - 1 > obj.Get_Index())
                obj.Set_Index(true);
            else
            {
                btn_next.IsEnabled = false;
                return;
            }

            Display_Info(current);
        }

        // Navigate to the previous item in the list.
        private void Button_Back(object sender, RoutedEventArgs e)
        {
            Package obj = new Package();
            Package current = new Package();

            try
            {
                current = Package.list[obj.Get_Index() - 1];
            }

            catch
            {
                Console.WriteLine("***Index is out of range for List of packages (Button_Back).");
                return;
            }

            obj.Set_Index(false);

            // Disable back button.
            if (obj.Get_Index() < 1)
            {
                btn_back.IsEnabled = false;

                if (obj.Get_Size() > 1)
                    btn_next.IsEnabled = true;
            }

            Display_Info(current);
        }

        // Display the information of the current package.
        private void Display_Info(Package current) {
            arrival.Text = current.arrival;
            pID.Text = current.pID.ToString();
            addr.Text = current.addr;
            city.Text = current.city;
            state.Text = current.state;
            zip.Text = current.zip.ToString();
        }

        // Reset the information output.
        private void Clear_Fields()
        {
            arrival.Text = "";
            pID.Text = "";
            addr.Text = "";
            city.Text = "";
            state.Text = "";
            zip.Text = "";
        }

        // Try to create a package using the input fields. 
        private Package Create_Package(Package package)
        {
            try
            {
                package = new Package(arrival.Text, int.Parse(pID.Text), addr.Text, city.Text, state.Text, int.Parse(zip.Text));
            }

            catch
            {
                Console.WriteLine("***Input is invalid for one or more fields (Create_Package).");
                return null;
            }

            return package;
        }

        // Display all package ID numbers destined for the state in the ListBox.
        private void State_DropDownClosed(object sender, EventArgs e)
        {
            // Find the selected state for each package.
            foreach (Package package in Package.list)
            {
                if (package.state == orderby.Text)
                    listbox.Items.Add(package.pID);

                // Remove listbox items that are not apart of the state.
                else if (package != null)
                    listbox.Items.Remove(package.pID);
            }
        }
    }
}