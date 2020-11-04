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

    TransformGroup transfromGroup = new TransformGroup();
    TranslateTransform translateTransform = new TranslateTransform();
    RotateTransform rotateTransform = new RotateTransform();

    public NewCompasso()
    {
      InitializeComponent();
      Loaded += NewCompasso_Loaded;
      PernaDireita.RenderTransform = new RotateTransform();
      PernaDireita.RenderTransformOrigin = new Point(0, 0);
      PernaEsquerda.RenderTransform = new RotateTransform();
      PernaEsquerda.RenderTransformOrigin = new Point(1, 0);
      transfromGroup.Children.Add(translateTransform);
      transfromGroup.Children.Add(rotateTransform);
      this.RenderTransform = transfromGroup;
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
      Rect PosDura = PontaDura.TransformToAncestor(this).TransformBounds(new Rect(PontaDura.RenderSize));
      Rect PosicaoPencil = PontaPencil.TransformToAncestor(this).TransformBounds(new Rect(PontaPencil.RenderSize));
      LinhaRaio.X1 = PosDura.X;
      LinhaRaio.Y1 = PosDura.Y;
      LinhaRaio.X2 = PosicaoPencil.X;
      LinhaRaio.Y2 = PosicaoPencil.Y;

      double TamanhoLinha = (LinhaRaio.X2 - LinhaRaio.X1) * 2;
      Circunferencia.Width = Circunferencia.Height = TamanhoLinha;
      Circunferencia.SetValue(Canvas.LeftProperty, (PosDura.X - Circunferencia.Width / 2));
      Circunferencia.SetValue(Canvas.TopProperty, (PosDura.Y - Circunferencia.Width / 2));
      UpdateTxtDura();
    }

    public void AdicionarPontoMarcacao()
    {
      Rect tmpPos = PontaDura.TransformToVisual(pai).TransformBounds(new Rect(PontaDura.RenderSize));
      PontoRotacao tmpNewPonto = new PontoRotacao();
      tmpNewPonto.SetValue(InkCanvas.LeftProperty, tmpPos.X);
      tmpNewPonto.SetValue(InkCanvas.TopProperty, tmpPos.Y);
      tmpNewPonto.SetValue(Panel.ZIndexProperty, 2000);
      tmpNewPonto.medidas.Text = $"{Math.Round(tmpPos.X)},{Math.Round(tmpPos.Y)}";
      pai.ic.Children.Add(tmpNewPonto);
    }

    public void AdicionarPontoMarcacao(Point p)
    {
      PontoRotacao tmpNewPonto = new PontoRotacao();
      tmpNewPonto.SetValue(InkCanvas.LeftProperty, p.X);
      tmpNewPonto.SetValue(InkCanvas.TopProperty, p.Y);
      tmpNewPonto.SetValue(Panel.ZIndexProperty, 2000);
      tmpNewPonto.medidas.Text = $"{Math.Round(p.X)},{Math.Round(p.Y)}";
      pai.ic.Children.Add(tmpNewPonto);
      Console.WriteLine(tmpNewPonto.medidas.Text);
    }

    public Rect PosicaoDura()
    {
      Rect tmpPos = PontaDura.TransformToVisual(this).TransformBounds(new Rect(PontaDura.RenderSize));
      return tmpPos;
    }

    private void thumbPernaDireita_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
    {
      pontoInicial = new Point(Mouse.GetPosition((UIElement)Parent).X, Mouse.GetPosition((UIElement)Parent).Y);
      //Console.WriteLine(pontoInicial);
    }

    private void thumbMove_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
      translateTransform.X += e.HorizontalChange;
      translateTransform.Y += e.VerticalChange;
      this.RenderTransform = transfromGroup;
      UpdateTxtDura();
    }

    Point center;
    double lastAngle;

    RotateTransform rotation;

    const int HANDLEMARGIN = 10;

    double TotalAngle, graus;
    private void thumbRotate_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
      InvalidateArrange();
      Rect PosDura = PosicaoDura();
      Point p = Mouse.GetPosition(pai);
      Rect tmpPosAtual = PosicaoCompasso;
      //AdicionarPontoMarcacao(p);

      double centerX = PosDura.X + tmpPosAtual.X;
      double centerY = PosDura.Y + tmpPosAtual.Y;
      //AdicionarPontoMarcacao(new Point(centerX, centerY));

      double rad = Math.Atan2(p.Y - centerY, p.X - centerX);
      int degrees = (int)(rad * 180 / Math.PI);
      graus = degrees;
      rotation = new RotateTransform(degrees, centerX - tmpPosAtual.X, centerY - tmpPosAtual.Y);
      RenderTransform = rotation;
      txtRender.Text = "" + degrees;
      Console.WriteLine($"Angulo={degrees} X={centerX} Y={centerY}");
    }

    private void thumbRotate_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
    {
      TotalAngle += graus;
    }

    public void UpdateTxtDura()
    {
      Rect Pos = PosicaoDura();
      txtPontaDura.Text = $"X={Math.Round(Pos.X)} Y={Math.Round(Pos.Y)}";
    }

    Rect PosicaoCompasso;
    private void thumbRotate_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
    {
      //center = new Point(strokeBounds.X + strokeBounds.Width / 2,strokeBounds.Y + strokeBounds.Height / 2);
      Rect PosDura = PosicaoDura();
      PosicaoCompasso = this.TransformToVisual(pai).TransformBounds(new Rect(this.RenderSize));
      center = new Point(PosDura.X, PosDura.Y);
      //AdicionarPontoMarcacao(center);
    }
  }
}
