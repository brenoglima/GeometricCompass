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

namespace Compasso
{
  /// <summary>
  /// Interaction logic for Compasso.xaml
  /// </summary>
  public partial class NewCompasso : UserControl
  {
    public NewCompasso()
    {
      InitializeComponent();
      Loaded += NewCompasso_Loaded;
      PernaDireita.RenderTransform = new RotateTransform();
      PernaDireita.RenderTransformOrigin = new Point(0, 0);
      PernaEsquerda.RenderTransform = new RotateTransform();
      PernaEsquerda.RenderTransformOrigin = new Point(1, 0);
    }

    private void NewCompasso_Loaded(object sender, RoutedEventArgs e)
    {
      UpdateLinhaRaio();
    }

    Point UltimoPonto;
    private void NewCompasso_MouseMovePernaDireita(object sender, MouseEventArgs e)
    {

    }

    Point pontoInicial;
    public MainWindow pai;
    private void thumbPernaDireita_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
      RotateTransform rtDireita = (RotateTransform)PernaDireita.RenderTransform;
      RotateTransform rtEsquerda = (RotateTransform)PernaEsquerda.RenderTransform;
      double PassoAngulo = 10;
      // Bloqueando Angulo Maximo
      double tmpAngulo = rtDireita.Angle - (e.HorizontalChange / PassoAngulo);
      if (Math.Abs(tmpAngulo) >= 360) { tmpAngulo = 0; }
      if (tmpAngulo <= -50) { tmpAngulo = -50; }
      if (tmpAngulo >= 10) { tmpAngulo = 10; }
      rtDireita.Angle = tmpAngulo;
      rtEsquerda.Angle = -tmpAngulo;
      txtAngulo.Text = Math.Round(rtDireita.Angle, 0).ToString() + "°";
      UpdateLinhaRaio();
      //Console.WriteLine("CHANGE=" + Math.Round(e.HorizontalChange, 2) + " | ANGULO=" + Math.Round(rtDireita.Angle, 2) + "   | TMPANGULO=" + tmpAngulo);
      //pai.InicialPos = PosicaoDura();
    }

    public void UpdateLinhaRaio()
    {
      Rect PosicaoDura = PontaDura.TransformToAncestor(this).TransformBounds(new Rect(PontaDura.RenderSize));
      Rect PosicaoPencil = PontaPencil.TransformToAncestor(this).TransformBounds(new Rect(PontaPencil.RenderSize));
      LinhaRaio.X1 = PosicaoDura.X;
      LinhaRaio.Y1 = PosicaoDura.Y;
      LinhaRaio.X2 = PosicaoPencil.X;
      LinhaRaio.Y2 = PosicaoPencil.Y;

      double TamanhoLinha = (LinhaRaio.X2 - LinhaRaio.X1) * 2;
      Circunferencia.Width = Circunferencia.Height = TamanhoLinha;
      Circunferencia.SetValue(Canvas.LeftProperty, (PosicaoDura.X - Circunferencia.Width / 2));
      Circunferencia.SetValue(Canvas.TopProperty, (PosicaoDura.Y - Circunferencia.Width / 2));

    }

    public void AdicionarPontoMarcacao()
    {
      Rect tmpPos = PontaDura.TransformToVisual(pai).TransformBounds(new Rect(PontaDura.RenderSize));
      PontoRotacao tmpNewPonto = new PontoRotacao();
      tmpNewPonto.SetValue(InkCanvas.LeftProperty, tmpPos.X);
      tmpNewPonto.SetValue(InkCanvas.TopProperty, tmpPos.Y);
      tmpNewPonto.SetValue(Panel.ZIndexProperty, 2000);
      tmpNewPonto.medidas.Text = $"{tmpPos.X},{tmpPos.Y}";
      pai.ic.Children.Add(tmpNewPonto);
    }

    public Rect PosicaoDura()
    {
      Rect tmpPos = PontaDura.TransformToVisual(this).TransformBounds(new Rect(PontaDura.RenderSize));
      return tmpPos;
    }

    private void thumbPernaDireita_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
    {
      pontoInicial = new Point(Mouse.GetPosition((Grid)Parent).X, Mouse.GetPosition((Grid)Parent).Y);
      //Console.WriteLine(pontoInicial);
    }

    private void GripPernaDireita_MouseDown(object sender, MouseButtonEventArgs e)
    {

    }
  }
}
