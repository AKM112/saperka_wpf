﻿using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace saperka;

public class Field : Button
{
    public static List<Field> Fields = new List<Field>();
    public static List<Field> Bombs = new List<Field>();
    public static int BombCount {get; private set;}
    public static int width;
    public static int height;
    public static List<Field> FieldsWithFlag = new List<Field>();
    public static int flagCount = 0;
    private static Action<int> OnFlagChanged;
    public int Value { get; set; }
    
    public static void Setup(Action<int> onFlagChanged, int bombCount)
    {
        BombCount = bombCount;
        OnFlagChanged = onFlagChanged;
        flagCount = 0;
        Fields.ForEach(x =>
        {
            x.Click += FistClick;
            x.Click -= odwroc;
            x.MouseRightButtonDown += RightClick;
        });
        
    }

    private static void RightClick(object sender, MouseButtonEventArgs e)
    {
        Field thisField = sender as Field;

        if (thisField.Content == "🚩")
        {
            thisField.Content = "";
            Field.flagCount--;
            if(Field.OnFlagChanged != null)
            Field.OnFlagChanged(flagCount);
        }
        else if (thisField.Content == "" && Field.flagCount < Field.BombCount)
        {
            thisField.Content = "🚩";
            Field.flagCount++;
            if(Field.OnFlagChanged != null)
                Field.OnFlagChanged(flagCount);
        }else if(Field.flagCount == Field.BombCount)
        {
            MessageBox.Show("Nie możesz oznaczyć więcej bomb niż ich jest!");
        }
    }

    public static void FistClick(object sender, RoutedEventArgs e)
    {
        Field field = (Field)sender;
        if (field.Content == "")
        {
            Field.Fields.ForEach(x =>
            {
                x.Click -= FistClick;
                x.Click += Field.odwroc;
            });
            int thisId = Field.Fields.IndexOf(field);
            setBombs(BombCount, thisId);
            updateBombs();
            odwroc(sender, e);   
        }
    }
    public static Field? getFieldAtPos(int x, int y)
    {
        if (x >= width || y >= height || x < 0 || y < 0)
            return null;
        int pos = width * y + x;
        if (pos < 0 || pos >= Fields.Count)
            return null;
        
        return Fields[pos];
    }

    public static bool UpdateAt(int x, int y)
    {
        Field field = getFieldAtPos(x, y);
        if (field != null)
        {
            if(field.Value!=10)
                field.Value += 1;
            return true;   
        }

        return false;
    }

    public static void odwroc(object sender, RoutedEventArgs e)
    {
        Field field = (Field)sender;
        
        if (field.Content == "🚩")
        {
            
        }
        else if (field.Value == 10)
        {
            MessageBoxButton retryTheSameBoard = MessageBoxButton.YesNo;
            showAllBombs();
            MessageBoxResult res =MessageBox.Show("Natrafiłeś na bombę!", "Czy chcesz zacząć na tej samej mapie?", MessageBoxButton.YesNo, MessageBoxImage.Stop);
            switch (res)
            {
                case MessageBoxResult.Yes:
                    Fields.ForEach(x => x.Content = "");
                    break;

                case MessageBoxResult.No:
                    MainWindow.newGame((MainWindow)Application.Current.MainWindow);
                    break;
            }

        }
        else
        {
            if (field.Value == 0)
            {
                int id = Fields.FindIndex(x => x == field);
                int x = id % width;
                int y = id / width;
                field.Content = field.Value.ToString();
                recOdwroc(x, y);
            }
            else
            {
                field.Content = field.Value.ToString();
            }

            int ileJestGit = ileZostaloZakrytych();
            int ileOflagowanychMaBombe = ileMaBombe();
            if (ileJestGit == Bombs.Count || ileOflagowanychMaBombe == Bombs.Count)
            {
                MessageBox.Show("Wygrałeś!", "Wygrana!", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow.newGame((MainWindow)Application.Current.MainWindow);
            }
        }
    }

    private static int ileMaBombe()
    {
        return Fields.Count(x => x.Content.ToString() == "🚩" && x.Value == 10);
    }

    private static int ileZostaloZakrytych()
    {
        int puste = 0;
        Fields.ForEach(x =>
        {
            if (x.Content == "" || x.Content == null || x.Content == "🚩")
            {
                puste += 1;
            }
        });
        return puste;
    }

    public static void recOdwroc(int x, int y)
    {
        for (int iT = x - 1; iT <= x + 1; iT++)
        {
            for (int jT = y - 1; jT <= y + 1; jT++)
            {
                if (!(x == iT && y == jT))
                {
                    Field f = getFieldAtPos(iT, jT);
                    if (f != null)
                    {
                        if (string.IsNullOrEmpty(f.Content?.ToString()) && f.Value == 0)
                        {
                            f.Content = f.Value.ToString();
                            recOdwroc(iT, jT);
                        }
                        else if(string.IsNullOrEmpty(f.Content?.ToString()))
                        {
                            f.Content = f.Value.ToString();
                        }
                    }
                }
            }
        }
    }

    private static void showAllBombs()
    {
        Fields.ForEach(
            x=> x.Content=x.Value.ToString());
        Bombs.ForEach(x =>
        {
            if(x.Content.ToString() == "🚩")
                x.Content = "🏳️";
            else
            x.Content = "💣";
        });
    }
    public static void setBombs(int howMany, int idWhereToNotPut)
    {
        BombCount = howMany;
        int ile = 0;
        List<int> freeSpace = new List<int>();
        
        Fields.ForEach(x =>
        { 
            freeSpace.Add(ile);
            Fields[ile].Value = 0;
            Fields[ile].Content = "";
            ile++;
        });
        
        freeSpace.Remove(idWhereToNotPut);
        
        Random random = new Random();
        for (int i = 0; i < BombCount; i++)
        {
            int id = random.Next(freeSpace.Count-1);
            if (Bombs.Contains(Fields[freeSpace[id]]))
            {
                i--;
                continue;
            }
            // Console.WriteLine("\nOJE: " + id);
            // Console.WriteLine("\n===================");
            // freeSpace.ForEach(x =>
            // {
            //     Console.Write(" " +x);
            // });
            Field bombPlace = Fields[freeSpace[id]];
            bombPlace.Value = 10;
            Bombs.Add(bombPlace);
            freeSpace.Remove(id);
        }
    }
    // 9 - bomba
    // 0 - puste pole

    public static void updateBombs()
    {
        for (int i = 0; i < Bombs.Count; i++)
        {
            int id = Fields.FindIndex(x => x == Bombs[i]);
            int x = id % width;
            int y = id / width;
            for (int iT = x - 1; iT <= x + 1; iT++)
            {
                for (int jT = y - 1; jT <= y + 1; jT++)
                {
                    if (!(x == iT && y == jT))
                    {
                        UpdateAt(iT, jT);
                    }
                }
            }
        }
    }
    public Field()
    {
        Value = 0;
    }
}