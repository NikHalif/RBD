using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RBD
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum IsStatusFind
        {
            All,
            FindIf,
        }


        enum IsStatusFilter
        {
            /// <summary>
            /// =
            /// </summary>
            Eq, 
            /// <summary>
            /// >
            /// </summary>
            Gt, 
            /// <summary>
            /// >=
            /// </summary>
            Gte,
            /// <summary>
            /// &lt;
            /// </summary>
            Lt,
            /// <summary>
            /// &lt;=
            /// </summary>
            Lte, 
            /// <summary>
            /// !=
            /// </summary>
            //Ne, 
        }

        MongoClient client;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void InitConnect()
        {
            client = new MongoClient(textBoxStrConnect.Text.Trim());
            Load(client);
        }

        private void Load(MongoClient _client)
        {
            if (client == null) return;
            PanelDB.Items.Clear();
            using (IAsyncCursor<BsonDocument> cursor = _client.ListDatabases())
            {
                while (cursor.MoveNext())
                {
                    foreach (var doc in cursor.Current)
                    {
                        // -- Создание основного интерфейса отображения
                        TabItem item = new TabItem();
                        item.Header = doc["name"];

                        Grid grid = new Grid();

                        ColumnDefinition colDef1 = new ColumnDefinition();
                        colDef1.Width = new GridLength(150);
                        ColumnDefinition colDef2 = new ColumnDefinition();
                        grid.ColumnDefinitions.Add(colDef1);
                        grid.ColumnDefinitions.Add(colDef2);

                        RowDefinition rowDef1 = new RowDefinition();
                        RowDefinition rowDef2 = new RowDefinition();
                        grid.RowDefinitions.Add(rowDef1);
                        grid.RowDefinitions.Add(rowDef2);

                        ListBox listBox = new ListBox();
                        Grid.SetColumn(listBox, 0);
                        Grid.SetRow(listBox, 0);
                        Grid.SetColumnSpan(listBox, 1);

                        ListBox listBoxResult = new ListBox();
                        Grid.SetRow(listBoxResult, 0);
                        Grid.SetColumn(listBoxResult, 1);
                        Grid.SetColumnSpan(listBoxResult, 1);

                        StackPanel stackPanel = new StackPanel();
                        stackPanel.Orientation = Orientation.Vertical;
                        stackPanel.VerticalAlignment = VerticalAlignment.Top;
                        stackPanel.Margin = new Thickness(5);
                        Grid.SetRow(stackPanel, 1);
                        Grid.SetColumn(stackPanel, 0);
                        Grid.SetColumnSpan(stackPanel, 2);

                        // -- Параметры поиска документов
                        IsStatusFind statusFind = IsStatusFind.All;

                        RadioButton radioButtonAllLoad = new RadioButton();
                        radioButtonAllLoad.GroupName = $"StatusLoad{doc["name"]}";
                        radioButtonAllLoad.Name = "FindAll";
                        radioButtonAllLoad.Content = "Вывод всех документов";
                        radioButtonAllLoad.IsChecked = true;
                        radioButtonAllLoad.MinHeight = 20;
                        radioButtonAllLoad.Checked += delegate (object sender, RoutedEventArgs e)
                        {
                            statusFind = IsStatusFind.All;
                        };
                        stackPanel.Children.Add(radioButtonAllLoad);

                        RadioButton radioButtonAllIf = new RadioButton();
                        radioButtonAllIf.GroupName = $"StatusLoad{doc["name"]}";
                        radioButtonAllIf.Name = "FindIf";
                        radioButtonAllIf.Content = "Вывод по условию";
                        radioButtonAllIf.MinHeight = 20;
                        radioButtonAllIf.Checked += delegate (object sender, RoutedEventArgs e)
                        {
                            statusFind = IsStatusFind.FindIf;
                        };
                        stackPanel.Children.Add(radioButtonAllIf);

                        StackPanel stackPanelFiler = new StackPanel();
                        stackPanelFiler.Orientation = Orientation.Horizontal;

                        StackPanel stackName = new StackPanel();
                        stackName.Orientation = Orientation.Vertical;
                        Label labelName = new Label();
                        labelName.Content = "Имя ключа";
                        stackName.Children.Add(labelName);
                        TextBox textName = new TextBox();
                        textName.MinWidth = 40;
                        stackName.Children.Add(textName);

                        stackPanelFiler.Children.Add(stackName);

                        ComboBox comboBox = new ComboBox();
                        comboBox.HorizontalAlignment = HorizontalAlignment.Center;
                        comboBox.VerticalAlignment = VerticalAlignment.Bottom;
                        comboBox.VerticalContentAlignment = VerticalAlignment.Center;
                        comboBox.Items.Add((new ComboBoxItem()).Content = "=");
                        comboBox.Items.Add((new ComboBoxItem()).Content = ">");
                        comboBox.Items.Add((new ComboBoxItem()).Content = ">=");
                        comboBox.Items.Add((new ComboBoxItem()).Content = "<");
                        comboBox.Items.Add((new ComboBoxItem()).Content = "<=");
                        stackPanelFiler.Children.Add(comboBox);

                        StackPanel stackValue = new StackPanel();
                        stackValue.Orientation = Orientation.Vertical;
                        Label labelValue = new Label();
                        labelValue.Content = "Значение";
                        stackValue.Children.Add(labelValue);

                        TextBox textValue = new TextBox();
                        textValue.MinWidth = 40;
                        stackValue.Children.Add(textValue);

                        stackPanelFiler.Children.Add(stackValue);

                        stackPanel.Children.Add(stackPanelFiler);

                        // -- Параметры сортировки документов
                        StackPanel stackSort = new StackPanel();
                        stackSort.Orientation = Orientation.Vertical;

                        StackPanel sortContentPalen = new StackPanel();
                        sortContentPalen.Orientation = Orientation.Horizontal;
                        sortContentPalen.VerticalAlignment = VerticalAlignment.Center;

                        Label labelSort = new Label(); 
                        labelSort.Content = "Сортировать по столбдцу";
                        sortContentPalen.Children.Add(labelSort);

                        TextBox textBoxSort = new TextBox();
                        textBoxSort.MinWidth = 30;
                        sortContentPalen.Children.Add(textBoxSort);


                        RadioButton radioButtonSortUp = new RadioButton();
                        radioButtonSortUp.GroupName = $"StatusSort{doc["name"]}";
                        radioButtonSortUp.Name = "SortUp";
                        radioButtonSortUp.Content = "Сортировка по возрастанию";
                        radioButtonSortUp.IsChecked = true;

                        RadioButton radioButtonSortDown = new RadioButton();
                        radioButtonSortDown.GroupName = $"StatusSort{doc["name"]}";
                        radioButtonSortDown.Name = "SortDown";
                        radioButtonSortDown.Content = "Сортировка по убыванию"; 

                        CheckBox CheckBoxSort = new CheckBox();
                        CheckBoxSort.Content = sortContentPalen;

                        stackSort.Children.Add(CheckBoxSort);
                        stackSort.Children.Add(radioButtonSortUp);
                        stackSort.Children.Add(radioButtonSortDown);

                        stackPanel.Children.Add(stackSort);

                        // -- Параметры отображаемых столбцов
                        StackPanel stackVis = new StackPanel();
                        stackVis.Orientation = Orientation.Vertical;

                        StackPanel VisContentPalen = new StackPanel();
                        VisContentPalen.Orientation = Orientation.Horizontal;
                        VisContentPalen.VerticalAlignment = VerticalAlignment.Center;

                        Label labelVis = new Label();
                        labelVis.Content = "Отобразить только ключи(через запятую):";
                        VisContentPalen.Children.Add(labelVis);

                        TextBox textBoxVis = new TextBox();
                        textBoxVis.MinWidth = 30;
                        VisContentPalen.Children.Add(textBoxVis);

                        CheckBox CheckBoxVis = new CheckBox();
                        CheckBoxVis.Content = VisContentPalen;

                        stackVis.Children.Add(CheckBoxVis);

                        stackPanel.Children.Add(stackVis);

                        // --- Кнопка обновления данных START
                        Button buttonUpdate = new Button();
                        buttonUpdate.Margin = new Thickness(10);
                        buttonUpdate.Content = "Обновить данные";
                        buttonUpdate.VerticalContentAlignment = VerticalAlignment.Bottom;

                        stackPanel.Children.Add(buttonUpdate);

                        // --- Загрузка таблиц
                        using (IAsyncCursor<BsonDocument> collectionCursor = _client.GetDatabase(doc["name"].ToString()).ListCollections())
                        {
                            while (collectionCursor.MoveNext())
                            {
                                foreach (var collDoc in collectionCursor.Current)
                                {
                                    listBox.Items.Add(collDoc["name"]);
                                }
                            }
                        }

                        List<BsonDocument> listResultFull = new List<BsonDocument>();
                        // --- Загрузка документов
                        void LoadDate(object sender, EventArgs e)
                        {
                            listBoxResult.Items.Clear();

                            var database = client.GetDatabase(doc["name"].ToString());
                            var collection = database.GetCollection<BsonDocument>(listBox.SelectedItem.ToString());
                            FilterDefinition<BsonDocument> filter = new BsonDocument();

                            if (statusFind == IsStatusFind.All){
                                filter = new BsonDocument();
                            }
                            else if(statusFind == IsStatusFind.FindIf)
                            {
                                if (comboBox.SelectedIndex == -1) { MessageBox.Show("Ошибка выбора условия фильтрации!"); return; }
                                string key = textName.Text.Trim(), value = textValue.Text.Trim();
                                int valueInt;
                                switch ((IsStatusFilter)comboBox.SelectedIndex)
                                {
                                    case IsStatusFilter.Eq:
                                        if(int.TryParse(value, out valueInt)) filter = Builders<BsonDocument>.Filter.Eq(key, valueInt);
                                        else filter = Builders<BsonDocument>.Filter.Eq(key, value);
                                        break;
                                    case IsStatusFilter.Gt:
                                        if (int.TryParse(value, out valueInt)) filter = Builders<BsonDocument>.Filter.Gt(key, valueInt);
                                        else filter = Builders<BsonDocument>.Filter.Gt(key, value);
                                        break;
                                    case IsStatusFilter.Gte:
                                        if (int.TryParse(value, out valueInt)) filter = Builders<BsonDocument>.Filter.Gte(key, valueInt);
                                        else filter = Builders<BsonDocument>.Filter.Gte(key, value);
                                        break;
                                    case IsStatusFilter.Lt:
                                        if (int.TryParse(value, out valueInt)) filter = Builders<BsonDocument>.Filter.Lt(key, valueInt);
                                        else filter = Builders<BsonDocument>.Filter.Lt(key, value);
                                        break;
                                    case IsStatusFilter.Lte:
                                        if (int.TryParse(value, out valueInt)) filter = Builders<BsonDocument>.Filter.Lte(key, valueInt);
                                        else filter = Builders<BsonDocument>.Filter.Lte(key, value);
                                        break;
                                    //case IsStatusFilter.Ne:
                                    //    filter = Builders<BsonDocument>.Filter.Ne
                                    //    break;
                                    default:
                                        MessageBox.Show("Ошибка выбора условия фильтрации!");
                                        break;
                                }
                            }

                            IFindFluent<BsonDocument, BsonDocument> people;

                            String textFilerName = textBoxSort.Text.Trim();
                            if (CheckBoxSort.IsChecked == true && textFilerName.Length > 0)
                            {
                                int iSort = 0;
                                if (radioButtonSortUp.IsChecked == true) iSort = 1;
                                else if(radioButtonSortDown.IsChecked == true) iSort = -1;
                                people = collection.Find(filter).Sort("{" + $"{textFilerName}:{iSort}" + "}");
                            }
                            else people = collection.Find(filter);

                            listResultFull = people.ToList();

                            textFilerName = textBoxVis.Text.Trim();
                            if (CheckBoxVis.IsChecked == true && textFilerName.Length > 0) people = people.Project("{" + GenerStr(textFilerName) + "}");

                            var tP = people.ToList();
                            foreach (var doc1 in tP)
                            {
                                listBoxResult.Items.Add(doc1);
                            }
                        };

                        buttonUpdate.Click += LoadDate;
                        listBox.SelectionChanged += LoadDate;

                        listBoxResult.MouseDoubleClick += delegate (object sender, System.Windows.Input.MouseButtonEventArgs e)
                        {
                            if (client == null || listBoxResult.SelectedIndex == -1 || listBoxResult.SelectedItems.Count > 1) return;
                            var tItem = (BsonDocument)listBoxResult.SelectedItem;

                            MessageBox.Show(listResultFull[listBoxResult.SelectedIndex].ToString());
                        };

                        grid.Children.Add(listBox);
                        grid.Children.Add(listBoxResult);
                        grid.Children.Add(stackPanel);
                        item.Content = grid;

                        PanelDB.Items.Add(item);
                    }
                }
            }
        }

        

        protected String GenerStr(String text)
        {
            var str = text.Trim().Split(',');
            if(str.Length <= 1) return $"{str[0].Trim()}:1,_id:0";
            else
            {
                String sTemp = "";
                foreach (var item in str)
                {
                    sTemp += $"{item.Trim()}:1,";
                }
                return sTemp+ "_id:0";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InitConnect();
        }
    }
}
