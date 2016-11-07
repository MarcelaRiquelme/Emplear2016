﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ObjectDraw.Figuras;

namespace ObjectDraw
{
  public class Editor : INotifyPropertyChanged
  {
    public static Editor Instancia { get; private set; }

    private Canvas _canvas;
    private Figura _figActual;
    private ObservableCollection<Figura> _objetos;

    public ICommand Mostrar { get; set; }

    public ICommand Ocultar { get; set; }

    public ICommand Mover { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    static Editor()
    {
      Instancia = new Editor();
    }

    private Editor()
    {
      _canvas = GetElementByName("mainArea", App.Current.MainWindow) as Canvas;

      if (_canvas != null)
        Console.WriteLine("Canvas encontrado con exito!");

      Objetos = new ObservableCollection<Figura>();

      Mostrar = new SimpleCommand(MostrarElementoActual, EstadoElementoActual);

      Ocultar = new SimpleCommand((o) =>
      {
        _canvas.Children.Remove(FiguraActual.Ocultar());
      }, (o) =>
      {
        if (FiguraActual == null)
          return false;
        return FiguraActual.Visible;
      });

      Mover = new SimpleCommand((o) =>
      {
        _canvas.Children.Remove(FiguraActual.Ocultar());
        FiguraActual.Mover(NewX, NewY);
        MostrarElementoActual(null);
      }, (o) => FiguraActual != null && FiguraActual.Visible);
  }

    private bool EstadoElementoActual(object obj)
    {
      if (FiguraActual == null)
        return false;
      return !FiguraActual.Visible;
    }

    private void MostrarElementoActual(object obj)
    {
      UIElement item = FiguraActual.Mostrar();
      _canvas.Children.Add(item);
      item.SetValue(Canvas.TopProperty, FiguraActual.Y);
      item.SetValue(Canvas.LeftProperty, FiguraActual.X);
    }

    public ObservableCollection<Figura> Objetos
    {
      get { return _objetos; }
      set
      {
        _objetos = value;
        OnPropertyChanged(nameof(Objetos));
      }
    }

    public Figura FiguraActual
    {
      get { return _figActual; }
      set
      {
        _figActual = value;
        OnPropertyChanged(nameof(FiguraActual));
      }
    }

    private double _x;
    public double NewX
    {
      get { return _x; }
      set
      {
        _x = value;
        OnPropertyChanged(nameof(NewX));
      }
    }

    private double _y;
    public double NewY
    {
      get { return _y; }
      set
      {
        _y = value;
        OnPropertyChanged(nameof(NewY));
      }
    }

    private void OnPropertyChanged(string prop)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    private FrameworkElement GetElementByName(string nombre, DependencyObject root)
    {
      FrameworkElement result = null;
      int hijos = VisualTreeHelper.GetChildrenCount(root);

      if (hijos >= 1)
      {
        //  obtener cada hijo de este objeto, chequear que sea el que tiene el nombre buscado
        //  si lo encuentro, lo retorno enseguida
        //  si no, continuo buscando
        for (int idx = 0; idx < hijos; idx++)
        {
          FrameworkElement probar = VisualTreeHelper.GetChild(root, idx) as FrameworkElement;

          if (probar != null)
          {
            //  o sea...si es un framework element....
            if (probar.Name == nombre)
            {
              result = probar;
            }
            else
            {
              probar = GetElementByName(nombre, probar);
              if (probar != null)
                result = probar;
            }
          }
        }
      }
      return result;
    }
  }
}
