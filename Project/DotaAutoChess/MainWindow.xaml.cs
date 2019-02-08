using System.Windows;
using System.Configuration;
using DotaAutoChess.DataClass;
using System.Collections.Generic;
using System;
using DotaAutoChess.Data;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Linq;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;

namespace DotaAutoChess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Global Variables
        private List<Hero> allHeroesList = new List<Hero>();
        private List<Specialization> allSpecializationsList = new List<Specialization>();
        private List<Rule> allRulesList = new List<Rule>();
        private List<DacColor> allColorsList = new List<DacColor>();

        private List<Hero> filteredHeroesList = new List<Hero>();
        //SetUp heroes array
        private Hero[] setupHeroArray = new Hero[10];

        //Rules to show
        private List<Rule> workingRulesList = new List<Rule>();

        //Filters (in setup)
        string name_filter = null;
        string cost_filter = null;
        string spec_filter1 = null;
        string spec_filter2 = null;
        TextBox tbFilterName = new TextBox();
        TextBox tbFilterCost = new TextBox();
        TextBox tbFilterSpec1 = new TextBox();
        TextBox tbFilterSpec2 = new TextBox();
        ComboBox cbFilterName = new ComboBox();
        ComboBox cbFilterCost = new ComboBox();
        ComboBox cbFilterSpec1 = new ComboBox();
        ComboBox cbFilterSpec2 = new ComboBox();


        //Draw Object mapping
        //SetUpGrid
        IDictionary<string, ColumnDefinition> dicSetUpColumnDefinition = new Dictionary<string, ColumnDefinition>();
        IDictionary<string, RowDefinition> dicSetUpRowDefinition = new Dictionary<string, RowDefinition>();
        IDictionary<string, ComboBox> dicSetUpComboBox = new Dictionary<string, ComboBox>();
        IDictionary<string, Button> dicSetUpDeleteButton = new Dictionary<string, Button>();
        IDictionary<string, TextBlock> dicSetUpTextFields = new Dictionary<string, TextBlock>();

        //BonusGrid
        //rules combined by same condition
        List<List<Rule>> rulesForGridList = new List<List<Rule>>();
        IDictionary<string, ColumnDefinition> dicBonusColumnDefinition = new Dictionary<string, ColumnDefinition>();
        IDictionary<string, RowDefinition> dicBonusRowDefinition = new Dictionary<string, RowDefinition>();
        IDictionary<string, TextBlock> dicBonusTextFields = new Dictionary<string, TextBlock>();

        //SaveNCloseGrid
        IDictionary<string, ColumnDefinition> dicSaveNCloseGridColumnDefinition = new Dictionary<string, ColumnDefinition>();
        IDictionary<string, RowDefinition> dicSaveNCloseGridRowDefinition = new Dictionary<string, RowDefinition>();

        //logBox
        TextBox logBox = new TextBox();

        //Sizes
        HeightWidth setUpHeaderGridSize;
        HeightWidth setUpGridSize;
        HeightWidth bonusGridSize;
        HeightWidth saveNLoadGridSize;
        HeightWidth filterGridSize;
        HeightWidth informationNLogGridSize;
        HeightWidth allHeroesGridSize;

        //Margins
        double leftGridsTopMarginForBonus;
        double leftGridsWidthForBonus;
        //double leftGridsTopMargin;
        //double leftGridsWidth;
        //double rightGridsWidth;
        //double rightGridsLeftMargin;
        //double rightGridsTopMargin;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Main();

        }

        /// <summary>
        /// Main...
        /// </summary>
        private void Main()
        {
            try
            {
                addLog("Let'go!");
                //Загрузка данных
                InitializeData();
                //Зарисовка поля
                DrawField();
            }
            catch (Exception ex)
            {
                addLog("Ошибка! " + ex.Message);
                MessageBox.Show("Ошибка! " + ex.Message);

            }
            finally
            {
                addLog("Combine your team!");
            }

        }

        /// <summary>
        /// Data Load
        /// </summary>
        private void InitializeData()
        {
            string heroesDatafile = ConfigurationManager.AppSettings["heroesDatafile"];
            allHeroesList = Hero.LoadFromCsv(heroesDatafile);
            addLog("Total " + allHeroesList.Count + " Heroes.");

            string specializationsDataFile = ConfigurationManager.AppSettings["specializationsDataFile"];
            allSpecializationsList = Specialization.LoadFromCsv(specializationsDataFile);
            addLog("Total " + allSpecializationsList.Count + " Specializations.");

            string rulesDatafile = ConfigurationManager.AppSettings["rulesDatafile"];
            allRulesList = Rule.LoadFromCsv(rulesDatafile);
            addLog("Total " + allRulesList.Count + " Bonuses.");

            string colorsDatafile = ConfigurationManager.AppSettings["colorsDatafile"];
            allColorsList = DacColor.LoadFromCsv(colorsDatafile);
            addLog("Total " + allColorsList.Count + " Colors.");


            filteredHeroesList = new List<Hero>(allHeroesList);

        }

        /// <summary>
        /// Draw all grids
        /// </summary>
        private void DrawField()
        {
            double leftGridsWidth = 500;
            double leftGridsTopMargin = 0;

            //<-------------left
            setUpHeaderGridSize = DrawSetUpHeaderGrid(leftGridsWidth, leftGridsTopMargin);

            //SETUP
            leftGridsTopMargin = setUpHeaderGridSize.Height + 5;
            setUpGridSize = DrawSetUpGrid(leftGridsWidth, leftGridsTopMargin);


            //SAVE OPEN
            leftGridsTopMargin = setUpHeaderGridSize.Height + setUpGridSize.Height + 10;
            saveNLoadGridSize = DrawSaveNLoadGrid(leftGridsWidth, leftGridsTopMargin);

            //BONUS
            leftGridsTopMargin = setUpHeaderGridSize.Height + setUpGridSize.Height + saveNLoadGridSize.Height + 15;
            bonusGridSize = DrawBonusGrid(leftGridsWidth, leftGridsTopMargin);



            //------------> right
            double rightGridsLeftMargin = leftGridsWidth + 5;
            double rightGridsWidth = 320;

            //FILTER
            filterGridSize = DrawFilterGrid(rightGridsWidth, rightGridsLeftMargin);

            //All Heroes
            double rightGridsTopMargin = filterGridSize.Height + 5;
            allHeroesGridSize = DrawAllheroesDG(rightGridsWidth, rightGridsLeftMargin, rightGridsTopMargin);



            //Globals for ReDraw
            leftGridsTopMarginForBonus = leftGridsTopMargin;
            leftGridsWidthForBonus = leftGridsWidth;

            //-------------bot
            //INFORMATION AND LOG
            //leftGridsTopMargin = setUpHeaderGridSize.Height + setUpGridSize.Height + saveNLoadGridSize.Height + bonusGridSize.Height + 20;
            informationNLogGridSize = DrawInformationNLogGrid(leftGridsWidth + rightGridsWidth + 5, rightGridsTopMargin + allHeroesGridSize.Height + 10);


            //----------------------
            double totalHeight = setUpHeaderGridSize.Height + setUpGridSize.Height + bonusGridSize.Height + saveNLoadGridSize.Height + informationNLogGridSize.Height + 200;
            double totalWidth = leftGridsWidth + +rightGridsWidth + 50;

            rootWindow.Height = totalHeight + 100;
            rootWindow.Width = totalWidth;
            rootGrid.Height = totalHeight;
            rootGrid.Width = totalWidth;



        }


        private HeightWidth DrawInformationNLogGrid(double inGridWidth, double leftGridsTopMargin)
        {

            double gridWidth = inGridWidth;
            int rowGridLength = 57;

            int rows = 1;
            //int columns = 2;

            Thickness margin = informationNLogGrid.Margin;
            margin.Top = leftGridsTopMargin;
            margin.Right = 0;
            margin.Left = 0;
            margin.Bottom = 0;
            informationNLogGrid.Margin = margin;

            informationNLogGrid.Width = gridWidth;
            informationNLogGrid.Height = rowGridLength * rows;

            informationNLogGrid.HorizontalAlignment = HorizontalAlignment.Left;
            informationNLogGrid.VerticalAlignment = VerticalAlignment.Top;

            //informationNLogGrid.ShowGridLines = true;



            #region Create Grid

            informationNLogGrid.ColumnDefinitions.Add(new ColumnDefinition { Name = "filterColumnDefinition" + 0 });
            informationNLogGrid.ColumnDefinitions.Add(new ColumnDefinition { Name = "filterColumnDefinition" + 1 });

            RowDefinition rowDefinition = new RowDefinition { Name = "filterDefinition" + 0 };
            rowDefinition.Height = new GridLength(rowGridLength);
            informationNLogGrid.RowDefinitions.Add(rowDefinition);

            #endregion

            #region Fill data manualy
            SolidColorBrush brush = GetDacColor(ColorConditionName.InformationText).Brush;

            //TextBox

            logBox.Name = "logBox";
            //logBox.Text = "INformation";
            logBox.FontWeight = FontWeights.Bold;
            logBox.Foreground = brush;
            logBox.VerticalAlignment = VerticalAlignment.Top;
            logBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            Grid.SetColumn(logBox, 0);
            Grid.SetRow(logBox, 0);
            informationNLogGrid.Children.Add(logBox);

            //Information
            TextBox tblName = new TextBox();
            tblName.IsReadOnly = true;
            tblName.TextWrapping = TextWrapping.Wrap;
            tblName.Name = "filterByNameLabel";
            tblName.Text = "Information:" + Environment.NewLine + @"https://www.reddit.com/user/superterentiev" + Environment.NewLine + @"superterentievdotaautochess@gmail.com";
            tblName.FontWeight = FontWeights.Bold;
            tblName.Foreground = brush;
            tblName.VerticalAlignment = VerticalAlignment.Top;
            tblName.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(tblName, 1);
            Grid.SetRow(tblName, 0);
            informationNLogGrid.Children.Add(tblName);
            #endregion


            return new HeightWidth(informationNLogGrid.Height, informationNLogGrid.Width);

        }

        private HeightWidth DrawSetUpHeaderGrid(double leftGridsWidth, double leftGridsTopMargin)
        {

            double gridWidth = leftGridsWidth;
            int rowGridLength = 25;

            int rows = 1;
            int columns = 4;



            Thickness margin = setUpHeaderGrid.Margin;
            margin.Top = leftGridsTopMargin;
            margin.Right = 0;
            margin.Left = 0;
            margin.Bottom = 0;
            setUpHeaderGrid.Margin = margin;

            setUpHeaderGrid.Width = gridWidth;
            setUpHeaderGrid.Height = rowGridLength * rows;

            setUpHeaderGrid.HorizontalAlignment = HorizontalAlignment.Left;
            setUpHeaderGrid.VerticalAlignment = VerticalAlignment.Top;

            setUpHeaderGrid.Background = GetDacColor(ColorConditionName.SetUpBackground).Brush;

            #region Create Grid

            ColumnDefinition columnDefinition = new ColumnDefinition { Name = "setUpHeaderGridColumnDefinition" + 0 };
            //columnDefinition.Width = new GridLength(gridWidth / 3);
            //setUpHeaderGrid.ColumnDefinitions.Add(columnDefinition);
            setUpHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Name = "setUpHeaderGridColumnDefinition" + 1 });
            setUpHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Name = "setUpHeaderGridColumnDefinition" + 2 });
            setUpHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Name = "setUpHeaderGridColumnDefinition" + 3 });


            RowDefinition rowDefinition = new RowDefinition { Name = "setUpHeaderGridRowDefinition" + 0 };
            rowDefinition.Height = new GridLength(rowGridLength);
            setUpHeaderGrid.RowDefinitions.Add(rowDefinition);
            #endregion

            #region Fill data manualy

            //ClearButton

            double columnWidth = leftGridsWidth / columns;


            Button button = new Button();
            button.Click += new RoutedEventHandler(ClearButton_Click);
            button.FontSize = 12;
            button.Content = "----";
            button.Width = columnWidth / 4;

            System.Drawing.Bitmap imageBit = DotaAutoChess.Properties.Resources.DeleteButton;
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(Path.Combine(Environment.CurrentDirectory, "Resources", "ClearSetUp.png")));
            StackPanel stackPnl = new StackPanel();
            stackPnl.Orientation = Orientation.Horizontal;
            stackPnl.Margin = new Thickness(1);
            stackPnl.Children.Add(img);
            button.Content = stackPnl;
            button.HorizontalAlignment = HorizontalAlignment.Left;

            //Tooltip
            ToolTip toolTip = new ToolTip();
            toolTip.Content = "Clear Setup";
            toolTip.StaysOpen = true;

            button.ToolTip = toolTip;

            Grid.SetRow(button, 0);
            Grid.SetColumn(button, 0);
            setUpHeaderGrid.Children.Add(button);


            //HEADER
            SolidColorBrush brush = GetDacColor(ColorConditionName.SetUpHeader).Brush;
            TextBlock tblHeader = new TextBlock();
            tblHeader.Name = "SetupHead";
            tblHeader.Text = "SetUp";
            tblHeader.FontWeight = FontWeights.Bold;
            tblHeader.Foreground = brush;
            tblHeader.VerticalAlignment = VerticalAlignment.Top;
            tblHeader.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(tblHeader, 1);
            Grid.SetRow(tblHeader, 0);
            setUpHeaderGrid.Children.Add(tblHeader);

            TextBlock mergeTb = new TextBlock();
            Grid.SetColumn(mergeTb, 2);
            Grid.SetRow(mergeTb, 0);
            setUpHeaderGrid.Children.Add(mergeTb);

            //Grid.SetColumnSpan(tblHeader, Grid.GetColumnSpan(tblHeader) + Grid.GetColumnSpan(mergeTb));
            //setUpHeaderGrid.Children.Remove(mergeTb);


            #endregion


            return new HeightWidth(setUpHeaderGrid.Height, setUpHeaderGrid.Width);


        }

        private HeightWidth DrawSaveNLoadGrid(double leftGridsWidth, double leftGridsTopMargin)
        {

            double gridWidth = leftGridsWidth;
            int rowGridLength = 30;

            int rows = 1;
            int columns = 3;

            Thickness margin = saveNLoadGrid.Margin;
            margin.Top = leftGridsTopMargin;
            margin.Right = 0;
            margin.Left = 0;
            margin.Bottom = 0;
            saveNLoadGrid.Margin = margin;

            saveNLoadGrid.Width = gridWidth;
            saveNLoadGrid.Height = rowGridLength * rows;

            saveNLoadGrid.HorizontalAlignment = HorizontalAlignment.Left;
            saveNLoadGrid.VerticalAlignment = VerticalAlignment.Top;

            saveNLoadGrid.ShowGridLines = true;





            #region Create Grid

            //GenerateName indexes
            List<string> namesRowsIdexesList = new List<string>();
            List<string> namesColumnsIdexesList = new List<string>();
            List<string> namesAllIdexesList = new List<string>();

            for (int irow = 0; irow < rows; irow++)
            {
                namesRowsIdexesList.Add("r" + irow);

                for (int icolumn = 0; icolumn < columns; icolumn++)
                {

                    namesAllIdexesList.Add("r" + irow + "c" + icolumn);
                }
            }
            for (int icolumn = 0; icolumn < columns; icolumn++)
            {
                namesColumnsIdexesList.Add("c" + icolumn);
            }



            // Create Columns
            foreach (string columnIndexName in namesColumnsIdexesList)
            {
                dicSaveNCloseGridColumnDefinition[columnIndexName] = new ColumnDefinition { Name = "saveNCloseColumnDefinition" + columnIndexName };
            }
            foreach (ColumnDefinition columnDefinition in dicSaveNCloseGridColumnDefinition.Values)
            {
                saveNLoadGrid.ColumnDefinitions.Add(columnDefinition);
            }


            // Create Rows

            foreach (string rowIndexName in namesRowsIdexesList)
            {
                dicSaveNCloseGridRowDefinition[rowIndexName] = new RowDefinition { Name = "saveNCloseRowDefinition" + rowIndexName };
            }
            foreach (RowDefinition rowDefinition in dicSaveNCloseGridRowDefinition.Values)
            {

                rowDefinition.Height = new GridLength(rowGridLength);
                saveNLoadGrid.RowDefinitions.Add(rowDefinition);

            }

            #endregion


            Button SaveButton = new Button();
            SaveButton.Name = "SaveButton";

            Image img = new Image();
            img.Source = new BitmapImage(new Uri(Path.Combine(Environment.CurrentDirectory, "Resources", "ChessQueen.png")));
            StackPanel stackPnl = new StackPanel();
            stackPnl.Orientation = Orientation.Horizontal;
            stackPnl.Margin = new Thickness(1);
            stackPnl.Children.Add(img);

            Label l1 = new Label();
            l1.Content = "  Save";
            stackPnl.Children.Add(l1);
            SaveButton.Content = stackPnl;


            //SelectionChanged
            SaveButton.Click += new RoutedEventHandler(SaveButton_Click);


            Grid.SetColumn(SaveButton, 0);
            Grid.SetRow(SaveButton, 0);
            saveNLoadGrid.Children.Add(SaveButton);

            //Load
            Button LoadButton = new Button();
            LoadButton.Name = "LoadButton";

            Image imgLoad = new Image();
            imgLoad.Source = new BitmapImage(new Uri(Path.Combine(Environment.CurrentDirectory, "Resources", "ChessRook.png")));
            StackPanel stackPnlLoad = new StackPanel();
            stackPnlLoad.Orientation = Orientation.Horizontal;
            stackPnlLoad.Margin = new Thickness(1);
            stackPnlLoad.Children.Add(imgLoad);

            Label l1Load = new Label();
            l1Load.Content = "  Load";
            stackPnlLoad.Children.Add(l1Load);
            LoadButton.Content = stackPnlLoad;


            //SelectionChanged
            LoadButton.Click += new RoutedEventHandler(LoadButton_Click);


            Grid.SetColumn(LoadButton, 1);
            Grid.SetRow(LoadButton, 0);
            saveNLoadGrid.Children.Add(LoadButton);



            return new HeightWidth(saveNLoadGrid.Height, saveNLoadGrid.Width);


        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.FileName = GenerateFileNameBySpec();
            save.Filter = "Text File | *.txt";
            string saveFolder = Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["saveFolder"]);
            try
            {
                Directory.CreateDirectory(saveFolder);
            }
            catch (Exception ex)
            {
                addLog(ex.Message);
            }
            save.InitialDirectory = saveFolder;

            if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(save.OpenFile());

                for (int i = 0; i < setupHeroArray.Length; i++)
                {
                    if (setupHeroArray[i] != null)
                    {
                        writer.WriteLine(setupHeroArray[i].Name + ";");
                    }
                }
                writer.Dispose();
                writer.Close();
            }
            addLog("File saved!");
        }

        private string GenerateFileNameBySpec()
        {
            string fileName = "";

            foreach (Rule rule in workingRulesList)
            {
                fileName += rule.HeroCobminations[0].Specialization.Name + "+";
            }

            if (fileName.Length > 1)
            {
                fileName = fileName.Remove(fileName.Length - 1);
            }
            fileName += ".txt";
            return fileName;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();

            string openFolder = Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["saveFolder"]);
            try
            {
                Directory.CreateDirectory(openFolder);
            }
            catch (Exception ex)
            {
                addLog(ex.Message);
            }
            openFileDialog.InitialDirectory = openFolder;
            openFileDialog.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            openFileDialog.Filter = "Text File | *.txt";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;
            // получаем выбранный файл

            string filename = openFileDialog.FileName;
            // читаем файл в строку
            string fileText = System.IO.File.ReadAllText(filename);

            var stringHeroes = fileText.Split(';');

            DeleteAllFromSetUP();

            foreach (var hero in stringHeroes)
            {

                setFreeComboboxByHero(hero.Trim());
            }

            addLog("File " + filename + " opened!");
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteAllFromSetUP();
        }

        private void DeleteAllFromSetUP()
        {
            for (int i = 0; i < 10; i++)
            {
                DeleteByRow(i + 1);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            int row = (Tools.Tools.CleanSymbols(button.Name));
            DeleteByRow(row);
        }

        private void DeleteByRow(int row)
        {
            var key = "r" + (row);
            dicSetUpComboBox[key].SelectedIndex = -1;

            foreach (var dicTextField in dicSetUpTextFields)
            {
                var keyText = dicTextField.Key;

                if (keyText.Contains("r" + row))
                {
                    var a = dicTextField.Value;
                    a.Text = "";

                }
            }
            setupHeroArray[row - 1] = null;
            SetUpHeroChanged();
            return;
        }

        
        #region Filters delete button Even handler
        private void DeleteFilterNameButton_Click(object sender, RoutedEventArgs e)
        {
            cbFilterName.SelectedIndex = -1;
            tbFilterName.Text = "";
        }
        private void DeleteFilterCostButton_Click(object sender, RoutedEventArgs e)
        {
            cbFilterCost.SelectedIndex = -1;
            tbFilterCost.Text = "";
        }
        private void DeleteFilterSpec1Button_Click(object sender, RoutedEventArgs e)
        {
            cbFilterSpec1.SelectedIndex = -1;
            tbFilterSpec1.Text = "";
        }
        private void DeleteFilterSpec2Button_Click(object sender, RoutedEventArgs e)
        {
            cbFilterSpec2.SelectedIndex = -1;
            tbFilterSpec2.Text = "";
        }
        #endregion


        private HeightWidth DrawFilterGrid(double rightGridsWidth, double rightGridsLeftMargin)
        {

            double gridWidth = rightGridsWidth;
            int rowGridLength = 25;

            int rows = 5; //4 filters + hint
            //int columns = 2;

            Thickness margin = filterGrid.Margin;
            margin.Top = 0;
            margin.Right = 0;
            margin.Left = rightGridsLeftMargin;
            margin.Bottom = 0;
            filterGrid.Margin = margin;

            filterGrid.Width = gridWidth;
            filterGrid.Height = rowGridLength * rows;

            filterGrid.HorizontalAlignment = HorizontalAlignment.Left;
            filterGrid.VerticalAlignment = VerticalAlignment.Top;

            //filterGrid.ShowGridLines = true;



            #region Create Grid

            ColumnDefinition columnDefinition = new ColumnDefinition { Name = "filterColumnDefinition" + 0 };
            columnDefinition.Width = new GridLength(gridWidth / 5);
            filterGrid.ColumnDefinitions.Add(columnDefinition);
            filterGrid.ColumnDefinitions.Add(new ColumnDefinition { Name = "filterColumnDefinition" + 1 });
            filterGrid.ColumnDefinitions.Add(new ColumnDefinition { Name = "filterColumnDefinition" + 2 });
            //clearbutton
            columnDefinition = new ColumnDefinition { Name = "filterColumnDefinition" + 3 };
            columnDefinition.Width = new GridLength(30);
            filterGrid.ColumnDefinitions.Add(columnDefinition);

            RowDefinition rowDefinition = new RowDefinition { Name = "filterDefinition" + 0 };
            rowDefinition.Height = new GridLength(rowGridLength);
            filterGrid.RowDefinitions.Add(rowDefinition);
            rowDefinition = new RowDefinition { Name = "filterDefinition" + 1 };
            rowDefinition.Height = new GridLength(rowGridLength);
            filterGrid.RowDefinitions.Add(rowDefinition);
            rowDefinition = new RowDefinition { Name = "filterDefinition" + 2 };
            rowDefinition.Height = new GridLength(rowGridLength);
            filterGrid.RowDefinitions.Add(rowDefinition);
            rowDefinition = new RowDefinition { Name = "filterDefinition" + 3 };
            rowDefinition.Height = new GridLength(rowGridLength);
            filterGrid.RowDefinitions.Add(rowDefinition);
            rowDefinition = new RowDefinition { Name = "filterDefinition" + 4 };
            rowDefinition.Height = new GridLength(rowGridLength);
            filterGrid.RowDefinitions.Add(rowDefinition);

            #endregion

            #region Fill data manually
            SolidColorBrush brush = GetDacColor(ColorConditionName.FilterLabel).Brush;

            //Filter Name
            TextBlock tblName = new TextBlock();
            tblName.Name = "filterByNameLabel";
            tblName.Text = "Name";
            tblName.FontWeight = FontWeights.Bold;
            tblName.Foreground = brush;
            tblName.VerticalAlignment = VerticalAlignment.Top;
            tblName.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(tblName, 0);
            Grid.SetRow(tblName, 0);
            filterGrid.Children.Add(tblName);

            tbFilterName.Name = "filterByName";
            tbFilterName.Text = "";
            Grid.SetColumn(tbFilterName, 1);
            Grid.SetRow(tbFilterName, 0);
            filterGrid.Children.Add(tbFilterName);

            tbFilterName.TextChanged += new TextChangedEventHandler(filterName_TextChanged);

            
            cbFilterName.Name = "cbFilterByName";
            Grid.SetColumn(cbFilterName, 2);
            Grid.SetRow(cbFilterName, 0);
            filterGrid.Children.Add(cbFilterName);
            //fill
            List<string> sortedList = new List<string>();
            foreach (Hero hero in allHeroesList)
            {
                sortedList.Add(hero.Name);
            }
            sortedList.Sort();
            foreach (string name in sortedList)
            {
                cbFilterName.Items.Add(name);
            }

            cbFilterName.SelectionChanged += new SelectionChangedEventHandler(ComboboxFilterNameChanged);
            //delete button
            addDeleteFilterButton(filterGrid, "Name", DeleteFilterNameButton_Click, 0, 4);


            //Filter Cost

            TextBlock tblCost = new TextBlock();
            tblCost.Name = "filterByCostLabel";
            tblCost.Text = "Cost";
            tblCost.FontWeight = FontWeights.Bold;
            tblCost.Foreground = brush;
            tblCost.VerticalAlignment = VerticalAlignment.Top;
            tblCost.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(tblCost, 0);
            Grid.SetRow(tblCost, 1);
            filterGrid.Children.Add(tblCost);


            tbFilterCost.Name = "filterByCost";
            tbFilterCost.Text = "";
            Grid.SetColumn(tbFilterCost, 1);
            Grid.SetRow(tbFilterCost, 1);
            filterGrid.Children.Add(tbFilterCost);
            tbFilterCost.TextChanged += new TextChangedEventHandler(filterCost_TextChanged);
            
            cbFilterCost.Name = "cbFilterByCost";
            Grid.SetColumn(cbFilterCost, 2);
            Grid.SetRow(cbFilterCost, 1);
            filterGrid.Children.Add(cbFilterCost);

            sortedList = new List<string>();
            foreach (Hero hero in allHeroesList)
            {
                if (!sortedList.Contains(hero.Cost.ToString()))
                {
                    sortedList.Add(hero.Cost.ToString());
                }
            }

            sortedList.Sort();
            foreach (string cost in sortedList)
            {
                cbFilterCost.Items.Add(cost);
            }

            cbFilterCost.SelectionChanged += new SelectionChangedEventHandler(ComboboxFilterCostChanged);
            //delete button
            addDeleteFilterButton(filterGrid, "Cost", DeleteFilterCostButton_Click, 1, 4);



            //Filter Spec1
            TextBlock tblSpec1 = new TextBlock();
            tblSpec1.Name = "filterBySpecLabel";
            tblSpec1.Text = "Spec1";
            tblSpec1.FontWeight = FontWeights.Bold;
            tblSpec1.Foreground = brush;
            tblSpec1.VerticalAlignment = VerticalAlignment.Top;
            tblSpec1.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(tblSpec1, 0);
            Grid.SetRow(tblSpec1, 2);
            filterGrid.Children.Add(tblSpec1);

            tbFilterSpec1.Name = "filterBySpec1";
            tbFilterSpec1.Text = "";
            Grid.SetColumn(tbFilterSpec1, 1);
            Grid.SetRow(tbFilterSpec1, 2);
            filterGrid.Children.Add(tbFilterSpec1);
            tbFilterSpec1.TextChanged += new TextChangedEventHandler(filterSpec1_TextChanged);

          
            cbFilterSpec1.Name = "cbFilterBySpec1";
            Grid.SetColumn(cbFilterSpec1, 2);
            Grid.SetRow(cbFilterSpec1, 2);
            filterGrid.Children.Add(cbFilterSpec1);
            sortedList = new List<string>();

            foreach (Hero hero in allHeroesList)
            {
                foreach (Specialization specialization in hero.Specializations)
                {
                    if (!sortedList.Contains(specialization.Name))
                    {
                        sortedList.Add(specialization.Name);
                    }
                }
            }
            sortedList.Sort();
            foreach (string spec in sortedList)
            {
                cbFilterSpec1.Items.Add(spec);
            }
            cbFilterSpec1.SelectionChanged += new SelectionChangedEventHandler(ComboboxFilterSpec1Changed);

            //delete button
            addDeleteFilterButton(filterGrid, "Spec1", DeleteFilterSpec1Button_Click, 2, 4);


            //Filter Spec2
            TextBlock tblSpec2 = new TextBlock();
            tblSpec2.Name = "filterBySpecLabel2";
            tblSpec2.Text = "Spec2";
            tblSpec2.FontWeight = FontWeights.Bold;
            tblSpec2.Foreground = brush;
            tblSpec2.VerticalAlignment = VerticalAlignment.Top;
            tblSpec2.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(tblSpec2, 0);
            Grid.SetRow(tblSpec2, 3);
            filterGrid.Children.Add(tblSpec2);


            tbFilterSpec2.Name = "filterBySpec2";
            tbFilterSpec2.Text = "";
            Grid.SetColumn(tbFilterSpec2, 1);
            Grid.SetRow(tbFilterSpec2, 3);
            filterGrid.Children.Add(tbFilterSpec2);
            tbFilterSpec2.TextChanged += new TextChangedEventHandler(filterSpec2_TextChanged);

           
            cbFilterSpec2.Name = "cbFilterBySpec2";
            Grid.SetColumn(cbFilterSpec2, 2);
            Grid.SetRow(cbFilterSpec2, 3);
            filterGrid.Children.Add(cbFilterSpec2);
            sortedList = new List<string>();

            foreach (Hero hero in allHeroesList)
            {
                foreach (Specialization specialization in hero.Specializations)
                {
                    if (!sortedList.Contains(specialization.Name))
                    {
                        sortedList.Add(specialization.Name);
                    }
                }
            }
            sortedList.Sort();
            foreach (string spec in sortedList)
            {
                cbFilterSpec2.Items.Add(spec);
            }
            cbFilterSpec2.SelectionChanged += new SelectionChangedEventHandler(ComboboxFilterSpec2Changed);
            //delete button
            addDeleteFilterButton(filterGrid, "Spec2", DeleteFilterSpec2Button_Click, 3, 4);


            //Hint To  dgAllHeroes
            brush = GetDacColor(ColorConditionName.HintForDgAllHeroes).Brush;

            TextBlock tbHint1 = new TextBlock();
            tbHint1.Name = "tbHint1";
            tbHint1 = setTextBlockRuleStyle(tbHint1, brush);
            tblCost.FontSize = 13;
            tbHint1.Text = "!You can Add Heroes to ←SetUp" + Environment.NewLine + "by double clicking on heroes below ↓!";
            Grid.SetColumn(tbHint1, 0);
            Grid.SetRow(tbHint1, 4);
            filterGrid.Children.Add(tbHint1);

            TextBlock tbHint2 = new TextBlock();
            Grid.SetColumn(tbHint2, 1);
            Grid.SetRow(tbHint2, 4);
            filterGrid.Children.Add(tbHint2);

            TextBlock tbHint3 = new TextBlock();
            Grid.SetColumn(tbHint3, 2);
            Grid.SetRow(tbHint3, 4);
            filterGrid.Children.Add(tbHint3);


            Grid.SetColumnSpan(tbHint1, Grid.GetColumnSpan(tbHint1) + Grid.GetColumnSpan(tbHint2));
            filterGrid.Children.Remove(tbHint2);
            Grid.SetColumnSpan(tbHint1, Grid.GetColumnSpan(tbHint1) + Grid.GetColumnSpan(tbHint3));
            filterGrid.Children.Remove(tbHint3);
            #endregion


            return new HeightWidth(filterGrid.Height, filterGrid.Width);

        }


        private void addDeleteFilterButton(Grid grid, string name, RoutedEventHandler evenFunc, int row, int column)
        {
            Button button = new Button();
            button.Name = "DeleteFilte" +name;
            button.Click += new RoutedEventHandler(evenFunc);
            button.FontSize = 12;
            button.Content = "delete";
            System.Drawing.Bitmap imageBit = DotaAutoChess.Properties.Resources.DeleteButton;
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(Path.Combine(Environment.CurrentDirectory, "Resources", "DeleteButton.jpg")));
            StackPanel stackPnl = new StackPanel();
            stackPnl.Orientation = Orientation.Horizontal;
            stackPnl.Margin = new Thickness(1);
            stackPnl.Children.Add(img);
            button.Content = stackPnl;

            ToolTip toolTip = new ToolTip();
            toolTip.Content = "Delete Filter";
            toolTip.StaysOpen = true;
            button.ToolTip = toolTip;

            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            grid.Children.Add(button);
        }

        private void filterName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            name_filter = txt.Text;
            FiltersBySteps();

        }

        private void filterCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            cost_filter = txt.Text;
            FiltersBySteps();
        }

        private void filterSpec1_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            spec_filter1 = txt.Text;
            FiltersBySteps();
        }
        private void filterSpec2_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            spec_filter2 = txt.Text;
            FiltersBySteps();
        }

        //Combobox filters changed
        void ComboboxFilterNameChanged(object sender, RoutedEventArgs e)
        {
            ComboBox curComboBox = (ComboBox)sender;
            if (curComboBox.SelectedIndex != -1)
            {
                var hero_name = curComboBox.SelectedValue.ToString();

                tbFilterName.Text += hero_name + ",";
                curComboBox.SelectedIndex = -1;
            }
        }

        void ComboboxFilterCostChanged(object sender, RoutedEventArgs e)
        {
            ComboBox curComboBox = (ComboBox)sender;
            if (curComboBox.SelectedIndex != -1)
            {
                var hero_cost = curComboBox.SelectedValue.ToString();

                tbFilterCost.Text += hero_cost + ",";
                curComboBox.SelectedIndex = -1;
            }
        }
        void ComboboxFilterSpec1Changed(object sender, RoutedEventArgs e)
        {
            ComboBox curComboBox = (ComboBox)sender;
            if (curComboBox.SelectedIndex != -1)
            {
                var hero_spec1 = curComboBox.SelectedValue.ToString();

                tbFilterSpec1.Text += hero_spec1 + ",";
                curComboBox.SelectedIndex = -1;
            }
        }
        void ComboboxFilterSpec2Changed(object sender, RoutedEventArgs e)
        {
            ComboBox curComboBox = (ComboBox)sender;
            if (curComboBox.SelectedIndex != -1)
            {
                var hero_spec2 = curComboBox.SelectedValue.ToString();

                tbFilterSpec2.Text += hero_spec2 + ",";
                curComboBox.SelectedIndex = -1;
            }
        }


        //MainFilter
        private void FiltersBySteps()
        {
            //add all heroes at first
            filteredHeroesList = new List<Hero>(allHeroesList);
            FilterHeroesByName(name_filter);
            //FilterHeroesByName(combo_name_filter);
            FilterHeroesByCost(cost_filter);
            FilterHeroesBySpec(spec_filter1, spec_filter2);

        }

        private void FilterHeroesByName(string Name)
        {
            if (String.IsNullOrEmpty(Name))
            {
                FilterChanged();
                return;
            }

            //split by comma
            string[] names = Name.Split(',');

            //What we got
            List<Hero> resultList = new List<Hero>();
            foreach (var findName in names)
            {
                if (!String.IsNullOrEmpty(findName))
                {
                    foreach (Hero hero in filteredHeroesList)
                    {
                        if (hero.Name.ToLower().Contains(findName.ToLower()))
                        {
                            if (!resultList.Contains(hero)) { resultList.Add(hero); }
                        }
                    }
                }
            }

            filteredHeroesList = new List<Hero>(resultList);
            FilterChanged();
        }

        private void FilterHeroesByCost(string Cost)
        {
            if (String.IsNullOrEmpty(Cost))
            {
                FilterChanged();
                return;
            }


            //split by comma
            string[] costs = Cost.Split(',');

            //What we got
            List<Hero> resultList = new List<Hero>();
            foreach (var cost in costs)
            {

                if (!String.IsNullOrEmpty(cost))
                {

                    int intCost = -1;
                    try
                    {
                        intCost = int.Parse(cost);
                    }
                    catch
                    {
                        addLog("Cost is a number...");
                    }

                    if (intCost > 0)
                    {

                        foreach (Hero hero in filteredHeroesList)
                        {
                            if (intCost > 0 && hero.Cost == intCost)
                            {
                                if (!resultList.Contains(hero)) { resultList.Add(hero); }
                            }
                        }

                    }
                }
            }

            filteredHeroesList = new List<Hero>(resultList);
            FilterChanged();
        }

        private void FilterHeroesBySpec(string Spec1, string Spec2)
        {

            List<Hero> resultHeroesList = new List<Hero>();

            List<Hero> spec1FilteredHeroesList = new List<Hero>();
            List<Hero> spec2FilteredHeroesList = new List<Hero>();

            //Spec1
            if (!String.IsNullOrEmpty(Spec1))
            {
                // split by comma
                string[] specs = Spec1.Split(',');


                // OR FILTER1
                foreach (var curSpec in specs)
                {
                    if (!String.IsNullOrEmpty(curSpec))
                    {
                        foreach (Hero hero in filteredHeroesList)
                        {
                            bool addHero = false;
                            foreach (Specialization specialization in hero.Specializations)
                            {
                                if (specialization.Name.ToLower().Contains(curSpec.ToLower()))
                                    addHero = true;
                            }
                            if (addHero)
                            {
                                spec1FilteredHeroesList.Add(hero);
                            }
                        }
                    }
                }
            }
            else
            {
                spec1FilteredHeroesList = new List<Hero>(filteredHeroesList);
            }



            //Spec2
            if (!String.IsNullOrEmpty(Spec2))
            {
                // split by comma
                string[] specs = Spec2.Split(',');

                // OR FILTER2
                foreach (var curSpec in specs)
                {
                    if (!String.IsNullOrEmpty(curSpec))
                    {
                        foreach (Hero hero in filteredHeroesList)
                        {
                            bool addHero = false;
                            foreach (Specialization specialization in hero.Specializations)
                            {
                                if (specialization.Name.ToLower().Contains(curSpec.ToLower()))
                                    addHero = true;
                            }
                            if (addHero)
                            {
                                spec2FilteredHeroesList.Add(hero);
                            }
                        }
                    }
                }
            }
            else
            {
                spec2FilteredHeroesList = new List<Hero>(filteredHeroesList);
            }

            foreach (Hero hero in spec1FilteredHeroesList)
            {
                if (spec2FilteredHeroesList.Contains(hero))
                {
                    if (!resultHeroesList.Contains(hero)) { resultHeroesList.Add(hero); }
                }

            }

            filteredHeroesList = new List<Hero>(resultHeroesList);
            FilterChanged();
        }

        private void FilterChanged()
        {
            dgAllHeroes.Items.Clear();
            FillAllHeroesDataGrid();
            dgAllHeroes.Items.Refresh();
        }

        private void FillAllHeroesDataGrid()
        {
            //Width="475" 
            dgAllHeroes.Width = 475;


            foreach (Hero hero in filteredHeroesList)
            {
                string spec1 = "";
                string spec2 = "";
                string spec3 = "";


                if (hero.Specializations.Count == 3)
                {
                    spec3 = hero.Specializations[2].Name;
                    spec2 = hero.Specializations[1].Name;
                    spec1 = hero.Specializations[0].Name;
                }
                if (hero.Specializations.Count == 2)
                {
                    spec2 = hero.Specializations[1].Name;
                    spec1 = hero.Specializations[0].Name;
                }
                if (hero.Specializations.Count == 1)
                {
                    spec1 = hero.Specializations[0].Name;
                }

                var data = new AllHeroesView { Name = hero.Name, Cost = hero.Cost.ToString(), Spec1 = spec1, Spec2 = spec2, Spec3 = spec3 };
                dgAllHeroes.Items.Add(data);

                dgAllHeroes.IsReadOnly = false;
                foreach (var column in dgAllHeroes.Columns)
                {
                    column.IsReadOnly = false;
                }
            }
        }

        /// <summary>
        /// Feill SetupGrid with Hero (if where is a free one)
        /// </summary>
        /// <param name="NameValue">Hero name</param>
        private void setFreeComboboxByHero(string NameValue)
        {
            for (int i = 0; i < setupHeroArray.Length; i++)
            {
                if (setupHeroArray[i] == null)
                {
                    var a = dicSetUpComboBox;
                    var key = "r" + (i + 1);
                    dicSetUpComboBox[key].SelectedValue = NameValue;
                    return;
                }
            }
        }


        private HeightWidth DrawAllheroesDG(double rightGridsWidth, double rightGridsLeftMargin, double rightGridsTopMargin)
        {
            //allHeroesGrid.Width = 320;

            allHeroesGrid.Width = rightGridsWidth;
            //allHeroesGrid.Height = setUpGridSize.Height + bonusGridSize.Height + saveNLoadGridSize.Height + 20 - filterGridSize.Height;//850; ((filteredHeroesList.Count+1) * 20);//setUpGridSize.Height + bonusGridSize.Height;

            Thickness margin = allHeroesGrid.Margin;
            margin.Top = rightGridsTopMargin;
            margin.Right = 0;//setUpGridSize.Width + 5;
            margin.Left = rightGridsLeftMargin;
            margin.Bottom = 0;
            allHeroesGrid.Margin = margin;

            //HeroesGrid
            var col1 = new DataGridTextColumn();
            col1.Header = "Name";
            col1.Binding = new Binding("Name");
            dgAllHeroes.Columns.Add(col1);

            var col2 = new DataGridTextColumn();
            col2.Header = "Cost";
            col2.Binding = new Binding("Cost");
            dgAllHeroes.Columns.Add(col2);

            var col3 = new DataGridTextColumn();
            col3.Header = "Spec1";
            col3.Binding = new Binding("Spec1");
            dgAllHeroes.Columns.Add(col3);

            var col4 = new DataGridTextColumn();
            col4.Header = "Spec2";
            col4.Binding = new Binding("Spec2");
            dgAllHeroes.Columns.Add(col4);

            var col5 = new DataGridTextColumn();
            col5.Header = "Spec3";
            col5.Binding = new Binding("Spec3");
            dgAllHeroes.Columns.Add(col5);


            FillAllHeroesDataGrid();

            dgAllHeroes.Items.Refresh();

            //Cancel editing for edit other objects from Datagrid handlers
            dgAllHeroes.BeginningEdit += Grid_BeginningEdit;


            return new HeightWidth(allHeroesGrid.Height, allHeroesGrid.Width);

        }

        private void dgAllHeroes_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //BUG?
            //addLog(sender.GetType().ToString());

            //var a = dgAllHeroes.CurrentCell.Column;
            //var b = dgAllHeroes.CurrentCell.Item;

            if (dgAllHeroes.SelectedItem == null) return;
            var heroView = dgAllHeroes.SelectedItem as AllHeroesView;
            string value = heroView.Name;
            setFreeComboboxByHero(value);
        }

        private void Grid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }


      //TODO
        /// <summary>
        /// Draw BonusGrid by code
        /// </summary>
        private HeightWidth DrawBonusGrid(double leftGridsWidth, double leftGridsTopMargin)
        {
            # region Clear Grid
            dicBonusColumnDefinition = new Dictionary<string, ColumnDefinition>();
            dicBonusRowDefinition = new Dictionary<string, RowDefinition>();
            dicBonusTextFields = new Dictionary<string, TextBlock>();
            rulesForGridList = new List<List<Rule>>();
            bonusGrid.Children.Clear();
            bonusGrid.RowDefinitions.Clear();
            bonusGrid.ColumnDefinitions.Clear();
            #endregion

            # region Get data for creating Grid

            foreach (Rule rule in allRulesList)
            {
                addRuleToBonusGrid(rulesForGridList, rule);
            }


            //Create table
            //getting number of rows and columns
            int rows = rulesForGridList.Count + workingRulesList.Count +1;//+header//NUMBER OF RULES WORKING!!!;

            int columns = 1;
            foreach (List<Rule> inListRule in rulesForGridList)
            {
                if (columns < (inListRule.Count + 1))
                {
                    columns = inListRule.Count + 1;
                }
            }
            #endregion

            //GridSettings
            double gridWidth = leftGridsWidth;
            int rowGridLength = 15;

            Thickness margin = bonusGrid.Margin;
            margin.Top = leftGridsTopMargin;
            margin.Right = 0;
            margin.Left = 0;
            margin.Bottom = 0;
            bonusGrid.Margin = margin;

            bonusGrid.Width = gridWidth;
            bonusGrid.Height = (rows * rowGridLength);

            bonusGrid.HorizontalAlignment = HorizontalAlignment.Left;
            bonusGrid.VerticalAlignment = VerticalAlignment.Top;

            //bonusGrid.ShowGridLines = true;



            #region Create Grid

            //GenerateName indexes
            List<string> namesRowsIdexesList = new List<string>();
            List<string> namesColumnsIdexesList = new List<string>();
            List<string> namesAllIdexesList = new List<string>();

            for (int irow = 0; irow < rows; irow++)
            {
                namesRowsIdexesList.Add("r" + irow);

                for (int icolumn = 0; icolumn < columns; icolumn++)
                {

                    namesAllIdexesList.Add("r" + irow + "c" + icolumn);
                }
            }
            for (int icolumn = 0; icolumn < columns; icolumn++)
            {
                namesColumnsIdexesList.Add("c" + icolumn);
            }



            // Create Columns
            foreach (string columnIndexName in namesColumnsIdexesList)
            {
                dicBonusColumnDefinition[columnIndexName] = new ColumnDefinition { Name = "bonusGridColumnDefinition" + columnIndexName };
            }
            foreach (ColumnDefinition columnDefinition in dicBonusColumnDefinition.Values)
            {
                if (Tools.Tools.CleanSymbols(columnDefinition.Name) == 0)
                {

                    columnDefinition.Width = new GridLength(250);
                }

                bonusGrid.ColumnDefinitions.Add(columnDefinition);

            }


            // Create Rows

            foreach (string rowIndexName in namesRowsIdexesList)
            {
                dicBonusRowDefinition[rowIndexName] = new RowDefinition { Name = "bonusGridRowDefinition" + rowIndexName };
            }
            foreach (RowDefinition rowDefinition in dicBonusRowDefinition.Values)
            {

                rowDefinition.Height = new GridLength(rowGridLength);
                bonusGrid.RowDefinitions.Add(rowDefinition);

            }

            #endregion

            //TODO

            #region Fill Grid
            //header
            TextBlock txtBlockHeader = new TextBlock();
            txtBlockHeader.Name = "bonusHeader";
            txtBlockHeader = setTextBlockRuleStyle(txtBlockHeader, GetDacColor(ColorConditionName.BonusHeader).Brush);
            txtBlockHeader.Text = "Hero (Count)";
            ToolTip toolTip = new ToolTip();
            toolTip.Content = "Same Heroes dont count for bonus!!";
            toolTip.StaysOpen = true;
            txtBlockHeader.ToolTip = toolTip;
            Grid.SetRow(txtBlockHeader, 0);
            Grid.SetColumn(txtBlockHeader, 0);
            bonusGrid.Children.Add(txtBlockHeader);



            for (int i =1; i< columns; i++)
            {
                TextBlock txtBlockHeaderNumber = new TextBlock();
                txtBlockHeaderNumber.Name = "bonusHeader" + i;
                txtBlockHeaderNumber = setTextBlockRuleStyle(txtBlockHeaderNumber, GetDacColor(ColorConditionName.BonusHeader).Brush);
                txtBlockHeaderNumber.Text = "Heroes Needed ";
                ToolTip toolTipNumber = new ToolTip();
                toolTipNumber.Content = "Needed number of heroes for bonus. Same Heroes dont count for bonus!!";
                toolTipNumber.StaysOpen = true;
                txtBlockHeaderNumber.ToolTip = toolTipNumber;
                Grid.SetRow(txtBlockHeaderNumber, 0);
                Grid.SetColumn(txtBlockHeaderNumber, i);
                bonusGrid.Children.Add(txtBlockHeaderNumber);
            }


            //Add TextFields

            int curRowIndex = 1;
            int curColumnIndex = 0;

            foreach (List<Rule> subList in rulesForGridList)
            {

                int addedRulesNumber = 1;

                foreach (Rule inListRule in subList)
                {
                    bool ruleWorks = workingRulesList.Contains(inListRule);
                    SolidColorBrush brush;
                    if (ruleWorks)
                    { brush = GetDacColor(ColorConditionName.BonusRuleWorking).Brush; }
                    else
                    { brush = GetDacColor(ColorConditionName.BonusRuleNotWorkingAtAll).Brush; }

                    string text = "";
                    string alreadyGotHeroesText = "";

                    string textSeparator;

                    switch (inListRule.ComninationCondition)
                    {
                        case ComninationCondition.Or:
                            textSeparator = " or ";
                            break;
                        case ComninationCondition.And:
                            textSeparator = " + ";
                            break;
                        default:
                                textSeparator = "";
                            break;

                    }


                    //Specialization
                    if (curColumnIndex == 0)
                    {

                        foreach (HeroCobmination heroCobmination in inListRule.HeroCobminations)
                        {
                            text += heroCobmination.Specialization.Name + textSeparator;

                            //Heroes with this specialiation exists
                            if (NumberOfDifferentSpecHeroesInSetup(heroCobmination.Specialization.Name) != 0 && !ruleWorks)
                            {
                                brush = GetDacColor(ColorConditionName.BonusRuleNearlyWorkingAtAll).Brush;
                            }
                            if (NumberOfDifferentSpecHeroesInSetup(heroCobmination.Specialization.Name) != 0)
                            {
                                alreadyGotHeroesText += NumberOfDifferentSpecHeroesInSetup(heroCobmination.Specialization.Name) + textSeparator;
                            }

                        }
                        if (alreadyGotHeroesText != "") {
                            if (textSeparator.Length >0)
                            {
                                alreadyGotHeroesText = "(" + alreadyGotHeroesText.Remove(alreadyGotHeroesText.Length - textSeparator.Length) + ")";
                            }
                            else
                            {
                                alreadyGotHeroesText = "(" + alreadyGotHeroesText + ")";
                            }
                            
                        }

                        if (textSeparator.Length > 0)
                        {
                            text = text.Remove(text.Length - textSeparator.Length);
                        }

                        text = text + "   " + alreadyGotHeroesText;

                        addTextBlockToGrid(bonusGrid, dicBonusTextFields, curRowIndex, curColumnIndex, text, brush);
                    }

                    curColumnIndex++;
                    text = "";
                    //NumberOfHeroes
                    foreach (HeroCobmination heroCobmination in inListRule.HeroCobminations)
                    {
                        text += heroCobmination.NumberOfHeroes + textSeparator;
                    }
                    if (textSeparator.Length > 0)
                    {
                        text = text.Remove(text.Length - textSeparator.Length);
                    }

                    addTextBlockToGrid(bonusGrid, dicBonusTextFields, curRowIndex, curColumnIndex, text, brush);

                    //Add bonus
                    if (ruleWorks)
                    {
                        string ruleBonus = inListRule.RuleBonus;
                        addBonusTextBlock(bonusGrid, dicBonusTextFields, (curRowIndex + addedRulesNumber), inListRule.RuleBonus, columns);
                        addedRulesNumber++;
                    }



                }
                //next rule
                curColumnIndex = 0;
                curRowIndex += addedRulesNumber;//depends on bonus
                addedRulesNumber = 1;
            }
            #endregion

            return new HeightWidth(bonusGrid.Height, bonusGrid.Width);

        }

        /// <summary>
        /// nu
        /// </summary>
        /// <param name="specialization"></param>
        /// <returns></returns>
        private int NumberOfDifferentSpecHeroesInSetup(string specialization)
        {
            //delete dublicates
            Hero[] processingSetupHeroList = new Hero[10];
            for (int i = 0; i < setupHeroArray.Length; i++)
            {
                Hero iHero = setupHeroArray[i];
                int pos = Array.IndexOf(processingSetupHeroList, iHero);

                //If not added
                if (!(pos > -1))
                {
                    processingSetupHeroList[i] = iHero;
                }
            }

            int result = 0;
            foreach (Hero hero in processingSetupHeroList)
            {
                if (hero != null)
                {
                    if (Hero.HeroGotSpec(hero, specialization))
                    {
                        result++;
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Add Textblock to grid
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="dicTextBlocks"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <param name="textValue"></param>
        private TextBlock addTextBlockToGrid(Grid grid, IDictionary<string, TextBlock> dicTextBlocks, int rowIndex, int columnIndex, string textValue, SolidColorBrush solidColorBrush)
        {

            string objectName = grid.Name + "textBlock" + "r" + rowIndex + "c" + columnIndex;
            string objecKey = "r" + rowIndex + "c" + columnIndex;
            dicTextBlocks[objecKey] = new TextBlock { Name = objectName };
            TextBlock txtBlock = dicTextBlocks[objecKey];


            txtBlock = setTextBlockRuleStyle(txtBlock, solidColorBrush);
            txtBlock.Text = textValue;

            Grid.SetRow(txtBlock, rowIndex);
            Grid.SetColumn(txtBlock, columnIndex);
            grid.Children.Add(txtBlock);

            return txtBlock;

        }

        /// <summary>
        /// Add cell merging all others in a row
        /// </summary>
        /// <param name="grid">Grid object</param>
        /// <param name="dicTextBlocks">Object dictionary</param>
        /// <param name="rowIndex">Row Index</param>
        /// <param name="textValue">Cell text</param>
        /// <param name="columns">NumberOfcolumns</param>
        private void addBonusTextBlock(Grid grid, IDictionary<string, TextBlock> dicTextBlocks, int rowIndex, string textValue, int columns)
        {

            //if write => rule works
            SolidColorBrush brush = GetDacColor(ColorConditionName.BonusRuleWorking).Brush;
            var bonusTextBlock = addTextBlockToGrid(grid, dicTextBlocks, rowIndex, 0, textValue, brush);

            //merging objects
            for (int i = 1; i < columns; i++)
            {
                var cellToMerge = addTextBlockToGrid(grid, dicTextBlocks, rowIndex, i, "merge", brush);
                Grid.SetColumnSpan(bonusTextBlock, Grid.GetColumnSpan(bonusTextBlock) + Grid.GetColumnSpan(cellToMerge));
                grid.Children.Remove(cellToMerge);
            }

        }

        private DacColor GetDacColor(ColorConditionName colorConditionName)
        {
            return DacColor.GetColorByEnumCondition(allColorsList, colorConditionName);
        }
        private DacColor GetDacColorByStr(string colorConditionName)
        {
            return DacColor.GetColorByEnumCondition(allColorsList, (ColorConditionName)Enum.Parse(typeof(ColorConditionName), colorConditionName));
        }

        //todo
        /// <summary>
        /// Add ruly To rulesForGridList
        /// </summary>
        /// <param name="rulesForGridList">list for making BonusGrid</param>
        /// <param name="rule">currnet rule</param>
        private void addRuleToBonusGrid(List<List<Rule>> rulesForGridList, Rule rule)
        {
            foreach (List<Rule> subList in rulesForGridList)
            {
                foreach (Rule inListRule in subList)
                {
                    //Process solo rules
                    //NOTHING
                    //Process DemonRules!
                    //if ((rule.HeroCobminations.Count == 1 && rule.HeroCobminations[0].Condition == BonusCondition.Solo))
                    //{
                    //    return;
                    //}

                    if (Rule.IsSameSpecRule(inListRule, rule))
                    {
                        subList.Add(rule);
                        return;
                    }
                }
            }
            //new rule
            rulesForGridList.Add(new List<Rule>() { rule });
        }

        /// <summary>
        /// Draw HeroSetUpGrid by code
        /// </summary>
        private HeightWidth DrawSetUpGrid(double leftGridsWidth, double leftGridsTopMargin)
        {


            //10  heroes + title
            int rows = 11;
            int columns = 6;

            int rowGridLength = 22;
            double gridWidth = leftGridsWidth;


            Thickness margin = setUpGrid.Margin;
            margin.Top = leftGridsTopMargin;
            margin.Right = 0;
            margin.Left = 0;
            margin.Bottom = 0;
            setUpGrid.Margin = margin;

            setUpGrid.Width = gridWidth;
            setUpGrid.Height = (rows * rowGridLength);

            setUpGrid.HorizontalAlignment = HorizontalAlignment.Left;

            setUpGrid.VerticalAlignment = VerticalAlignment.Top;
            setUpGrid.Background = GetDacColor(ColorConditionName.SetUpBackground).Brush;
            setUpGrid.ShowGridLines = false;


            #region Create Grid
            //GenerateName indexes
            List<string> namesRowsIdexesList = new List<string>();
            List<string> namesColumnsIdexesList = new List<string>();
            List<string> namesAllIdexesList = new List<string>();
            for (int irow = 0; irow < rows; irow++)
            {
                namesRowsIdexesList.Add("r" + irow);

                for (int icolumn = 0; icolumn < columns; icolumn++)
                {

                    namesAllIdexesList.Add("r" + irow + "c" + icolumn);
                }
            }
            for (int icolumn = 0; icolumn < columns; icolumn++)
            {
                namesColumnsIdexesList.Add("c" + icolumn);
            }
            // Create Columns

            foreach (string columnIndexName in namesColumnsIdexesList)
            {
                dicSetUpColumnDefinition[columnIndexName] = new ColumnDefinition { Name = "columnDefinition" + columnIndexName };

            }
            foreach (ColumnDefinition columnDefinition in dicSetUpColumnDefinition.Values)
            {
                //delete button
                if (Tools.Tools.CleanSymbols(columnDefinition.Name) == 0)
                {

                    columnDefinition.Width = new GridLength(30);
                }
                //hero name
                if (Tools.Tools.CleanSymbols(columnDefinition.Name) == 1)
                {

                    columnDefinition.Width = new GridLength(150);
                }

                setUpGrid.ColumnDefinitions.Add(columnDefinition);
            }


            // Create Rows

            foreach (string rowIndexName in namesRowsIdexesList)
            {
                dicSetUpRowDefinition[rowIndexName] = new RowDefinition { Name = "rowDefinition" + rowIndexName };
            }
            foreach (RowDefinition rowDefinition in dicSetUpRowDefinition.Values)
            {


                rowDefinition.Height = new GridLength(rowGridLength);
                setUpGrid.RowDefinitions.Add(rowDefinition);

            }
            #endregion

            #region Fill Grid
            //add delete button
            foreach (string rowIndexName in namesRowsIdexesList)
            {
                dicSetUpDeleteButton[rowIndexName] = new Button { Name = "deleteButton" + rowIndexName };
            }

            foreach (KeyValuePair<string, Button> dicItems in dicSetUpDeleteButton)
            {
                int rowNumber = int.Parse(dicItems.Key.Replace("r", ""));

                if (rowNumber != 0)
                {
                    Button button = dicItems.Value;
                    button.Click += new RoutedEventHandler(DeleteButton_Click);
                    button.FontSize = 12;
                    button.Content = "---";

                    System.Drawing.Bitmap imageBit = DotaAutoChess.Properties.Resources.DeleteButton;

                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(Path.Combine(Environment.CurrentDirectory, "Resources", "DeleteButton.jpg")));
                    StackPanel stackPnl = new StackPanel();
                    stackPnl.Orientation = Orientation.Horizontal;
                    stackPnl.Margin = new Thickness(1);
                    stackPnl.Children.Add(img);
                    button.Content = stackPnl;

                    ToolTip toolTip = new ToolTip();
                    toolTip.Content = "Delete Hero";
                    toolTip.StaysOpen = true;
                    button.ToolTip = toolTip;

                    Grid.SetRow(button, rowNumber);
                    Grid.SetColumn(button, 0);
                    setUpGrid.Children.Add(button);
                }
            }


            //Add comboboxes

            foreach (string rowIndexName in namesRowsIdexesList)
            {
                dicSetUpComboBox[rowIndexName] = new ComboBox { Name = "comboBox" + rowIndexName };
            }
            foreach (KeyValuePair<string, ComboBox> dicItems in dicSetUpComboBox)
            {
                int rowNumber = int.Parse(dicItems.Key.Replace("r", ""));

                if (rowNumber != 0)
                {
                    ComboBox comboBox = dicItems.Value;
                    foreach (Hero hero in allHeroesList)
                    {
                        comboBox.Items.Add(hero.Name);
                    }

                    comboBox.SelectionChanged += new SelectionChangedEventHandler(HeroFormSetUpChanged);
                    comboBox.FontSize = 12;
                    Grid.SetRow(comboBox, rowNumber);
                    Grid.SetColumn(comboBox, 1);
                    setUpGrid.Children.Add(comboBox);
                }
            }

            //Add TextFields
            foreach (string rowIndexName in namesRowsIdexesList)
            {
                foreach (string columnIndexName in namesColumnsIdexesList)
                {
                    dicSetUpTextFields[rowIndexName + columnIndexName] = new TextBlock { Name = "textBlock" + rowIndexName + columnIndexName };
                }
            }

            foreach (TextBlock txtBlock in dicSetUpTextFields.Values)
            {

                int rowIndex = Tools.Tools.GetRowIndex(txtBlock);
                int columnIndex = Tools.Tools.GetColumnIndex(txtBlock);

                if (rowIndex > 0 && columnIndex > 1)
                {

                    txtBlock.FontSize = 12;
                    txtBlock.FontWeight = FontWeights.Bold;
                    txtBlock.Foreground = new SolidColorBrush(Colors.DarkBlue);
                    txtBlock.VerticalAlignment = VerticalAlignment.Top;
                    txtBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    txtBlock.Text = "";
                    Grid.SetRow(txtBlock, rowIndex);
                    Grid.SetColumn(txtBlock, columnIndex);
                    setUpGrid.Children.Add(txtBlock);
                }
            }


            // Add Headers
            //addHeaderToSetupGrid(0, "Setup");
            addHeaderToSetupGrid(1, "Hero Name ↕");
            addHeaderToSetupGrid(2, "Cost ↕");
            addHeaderToSetupGrid(3, "Spec1 ↕");
            addHeaderToSetupGrid(4, "Spec2 ↕");
            addHeaderToSetupGrid(5, "Spec3 ↕");
            #endregion

            return new HeightWidth(setUpGrid.Height, setUpGrid.Width);

        }

        private void addHeaderToSetupGrid(int columnIndex, string text)
        {

            TextBlock txtBlock = new TextBlock { Name = "textBlockSetUpTitle" + columnIndex };
            txtBlock.Text = text;
            txtBlock = setTextBlockTitleStyle(txtBlock);

            txtBlock.PreviewMouseDown += new MouseButtonEventHandler(SetUpHeaderSort);
            Grid.SetRow(txtBlock, 0);
            Grid.SetColumn(txtBlock, columnIndex);
            setUpGrid.Children.Add(txtBlock);

        }


        void SetUpHeaderSort(object sender, MouseButtonEventArgs e)
        {


            TextBlock curTextBlock = (TextBlock)sender;
            SortSetUpList(curTextBlock.Text);
        }

        void SortSetUpList(string sortField)
        {
            List<Hero> sortedSetupHeroList = new List<Hero>();

            for (int i = 0; i < setupHeroArray.Length; i++)
            {
                if (setupHeroArray[i] != null)
                    sortedSetupHeroList.Add(setupHeroArray[i]);
            }

            if (sortField.Contains("↕")) { sortField = sortField.Remove(sortField.Length - 2); }

            Hero[] sortedSetupHeroArray;
            switch (sortField)
            {

                case "Hero Name":
                    sortedSetupHeroArray = sortedSetupHeroList.OrderBy(s => s.Name).ToArray();
                    break;
                case "Cost":
                    sortedSetupHeroArray = sortedSetupHeroList.OrderBy(s => s.Cost).ToArray();
                    break;
                case "Spec1":
                    sortedSetupHeroArray = sortedSetupHeroList.OrderBy(s => s.Specializations[0].Name).ToArray();
                    break;
                case "Spec2":
                    sortedSetupHeroArray = sortedSetupHeroList.OrderBy(s => s.Specializations[1].Name).ToArray();
                    break;
                case "Spec3":
                    sortedSetupHeroArray = sortedSetupHeroList.OrderBy(s => (s.Specializations.Count == 3 ? s.Specializations[2].Name : "")).ToArray();
                    break;

                default:
                    return;
            }

            //delete all
            for (int i = 0; i < setupHeroArray.Length; i++)
            {
                DeleteByRow(i + 1);
            }

            //set all
            for (int i = 0; i < sortedSetupHeroArray.Length; i++)
            {
                string value = sortedSetupHeroArray[i].Name;
                setFreeComboboxByHero(value);
            }

            //update setupHeroArray
            for (int i = 0; i < sortedSetupHeroArray.Length; i++)
            {
                setupHeroArray[i] = sortedSetupHeroArray[i];
            }

            for (int i = sortedSetupHeroArray.Length; i < setupHeroArray.Length; i++)
            {
                setupHeroArray[i] = null;
            }
        }


        private TextBlock setTextBlockTitleStyle(TextBlock titleTextblock)
        {
            titleTextblock.FontSize = 12;
            titleTextblock.FontWeight = FontWeights.Bold;
            titleTextblock.Foreground = GetDacColor(ColorConditionName.SetUpColumnsHeader).Brush;
            titleTextblock.VerticalAlignment = VerticalAlignment.Top;
            titleTextblock.HorizontalAlignment = HorizontalAlignment.Center;
            return titleTextblock;
        }

        private TextBlock setTextBlockRuleStyle(TextBlock titleTextblock, SolidColorBrush solidColorBrush)
        {
            titleTextblock.FontSize = 10;
            titleTextblock.FontWeight = FontWeights.Bold;
            titleTextblock.Foreground = solidColorBrush;
            titleTextblock.VerticalAlignment = VerticalAlignment.Top;
            titleTextblock.HorizontalAlignment = HorizontalAlignment.Center;
            return titleTextblock;
        }


        void HeroFormSetUpChanged(object sender, RoutedEventArgs e)
        {
            ComboBox curComboBox = (ComboBox)sender;
            if (curComboBox.SelectedIndex != -1)
            {
                FillHeroSlotByCombobox(curComboBox);
            }
            SetUpHeroChanged();
        }

        /// <summary>
        /// Fill Hero Slot
        /// </summary>
        /// <param name="curComboBox"></param>
        void FillHeroSlotByCombobox(ComboBox curComboBox)
        {
            int rowNumber = Tools.Tools.CleanSymbols(curComboBox.Name);
            Hero curHero = Hero.GetHeroByName(allHeroesList, curComboBox.SelectedItem.ToString());

            SolidColorBrush brush = GetDacColor(ColorConditionName.Black).Brush;

            //Fill textBlocks
            //Cost
            string key = "r" + rowNumber + "c" + 2;
            var txtBlock = dicSetUpTextFields[key];
            txtBlock.Text = curHero.Cost.ToString();
            txtBlock.Foreground = brush;



            //Spec1
            key = "r" + rowNumber + "c" + 3;
            txtBlock = dicSetUpTextFields[key];
            txtBlock.Text = curHero.Specializations[0].Name;
            txtBlock.Foreground = GetDacColorByStr(txtBlock.Text).Brush;

            ToolTip toolTip1 = new ToolTip();
            toolTip1.Content = Rule.GetSpecializationForHint(allRulesList, txtBlock.Text);
            toolTip1.StaysOpen = true;
            txtBlock.ToolTip = toolTip1;

            //Spec2
            key = "r" + rowNumber + "c" + 4;
            txtBlock = dicSetUpTextFields[key];
            if (curHero.Specializations.Count > 1)
            {
                txtBlock.Text = curHero.Specializations[1].Name;
                txtBlock.Foreground = GetDacColorByStr(txtBlock.Text).Brush;

                ToolTip toolTip2 = new ToolTip();
                toolTip2.Content = Rule.GetSpecializationForHint(allRulesList, txtBlock.Text);
                toolTip2.StaysOpen = true;
                txtBlock.ToolTip = toolTip2;


            }

            //Spec3
            key = "r" + rowNumber + "c" + 5;
            txtBlock = dicSetUpTextFields[key];
            if (curHero.Specializations.Count > 2)
            {
                txtBlock.Text = curHero.Specializations[2].Name;
                txtBlock.Foreground = GetDacColorByStr(txtBlock.Text).Brush;
                ToolTip toolTip2 = new ToolTip();
                toolTip2.Content = Rule.GetSpecializationForHint(allRulesList, txtBlock.Text);
                toolTip2.StaysOpen = true;
                txtBlock.ToolTip = toolTip2;
            }

            // System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(string name);


            //SetUp heroes array fill
            setupHeroArray[rowNumber - 1] = curHero;
        }

        void SetUpHeroChanged()
        {

            workingRulesList = new List<Rule>();

            foreach (List<Rule> subList in rulesForGridList)
            {
                foreach (Rule inListRule in subList)
                {

                    if (Rule.RuleWorks(inListRule, setupHeroArray))
                    {
                        //RULE WORKS!
                        workingRulesList.Add(inListRule);
                    }
                }
            }

            //перерисовываем правила!
            DrawBonusGrid(leftGridsWidthForBonus, leftGridsTopMarginForBonus);


        }

        /// <summary>
        /// Логирование важных событий в строке для пользователя.
        /// </summary>
        /// <param name="logLine">Текст для отображения</param>
        private void addLog(string logLine)
        {
            logBox.Text = logLine + Environment.NewLine + logBox.Text;
            //logBox.AppendText(logLine + Environment.NewLine);
        }

        /// <summary>
        /// Класс для отоборажения в таблице героев
        /// </summary>
        private class AllHeroesView
        {
            public string Name { get; set; }
            public string Cost { get; set; }
            public string Spec1 { get; set; }
            public string Spec2 { get; set; }
            public string Spec3 { get; set; }
        }

        /// <summary>
        /// Пара переменных для описания размеров
        /// </summary>
        private class HeightWidth
        {
            public double Height;
            public double Width;

            public HeightWidth(double height, double width)
            {
                this.Height = height;
                this.Width = width;
            }
        }


    }
}
