using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Compasso
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    ToolsCompasso CompassoAntigo;
    public MainWindow()
    {
      CompassoAntigo = new ToolsCompasso();
      InitializeComponent();
      Loaded += MainWindow_Loaded;
      //CompassoAntigo.RenderTransform = new MatrixTransform(new Matrix());
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      CompassoNovo.pai = this;
      CompassoNovo.RenderTransform = rt;
      CompassoAntigo.knobRotate.MouseDown += KnobRotate_MouseDown;

      //grdMain.MouseMove += Ic_MouseMoveRotating;
      //grdMain.MouseMove += Escrever;
      //grdMain.MouseUp += Ic_MouseUp;

    }

    StylusPointCollection spc;
    private void Escrever(object sender, MouseEventArgs e)
    {
      //Stroke s = new Stroke();
      if (isRotating)
      {
        Rect PosicaoPencil = CompassoAntigo.Pencil.TransformToAncestor(this).TransformBounds(new Rect(CompassoAntigo.Pencil.RenderSize));
        StylusPoint p = new StylusPoint(PosicaoPencil.X, PosicaoPencil.Y);
        spc.Add(p);
      }
    }

    private void Ic_MouseUp(object sender, MouseButtonEventArgs e)
    {
      if (isRotating)
      {
        isRotating = false;
        TotalAngle = CurrentAngle;
      }
      //ic.EditingMode = InkCanvasEditingMode.EraseByPoint;
    }

    private void Ic_MouseMoveRotating(object sender, MouseEventArgs e)
    {
      if (isRotating)
      {
        var _container = ic;
        // Restaura posicao da Matrix, aproveitando Scale, Translation e Rotation
        Matrix mAtual = ((MatrixTransform)RenderTransform).Matrix;

        // Calcula o angulo atual da posição do mouse
        double dx = e.GetPosition(_container).X - (NovaPosicao.X + (NovaPosicao.Width / 2));
        double dy = e.GetPosition(_container).Y - (NovaPosicao.Y + (NovaPosicao.Height / 2));
        double new_angle = Math.Atan2(dy, dx);
        // Calcula a mudança do angulo
        CurrentAngle = new_angle - StartAngle;
        // Converte em angulos
        CurrentAngle *= 180 / (float)Math.PI;
        // Soma ao angulo atual o angulo acumulado
        CurrentAngle += TotalAngle;

        // Calculando diferença entre angulo total e angulo já girado e gira no sentido contrario
        var x = new Vector(1, 0);
        Vector rotated = Vector.Multiply(x, mAtual);
        double angleBetween = Vector.AngleBetween(x, rotated);

        Rect medCompasso = CompassoAntigo.TransformToAncestor(this).TransformBounds(new Rect(CompassoAntigo.RenderSize));
        mAtual.Rotate(-angleBetween);
        mAtual.Rotate(CurrentAngle);
        CompassoAntigo.RenderTransform = new MatrixTransform(mAtual);
      }
    }

    double StartAngle, CurrentAngle, TotalAngle = 0;
    Rect NovaPosicao;
    bool isRotating;

    int angulo = 0;
    public Rect InicialPos;
    RotateTransform rt = new RotateTransform();
    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Rect PosDura = CompassoNovo.PosicaoDura();
      rt.CenterX = PosDura.X;
      rt.CenterY = PosDura.Y;
      rt.Angle += 15;
    }

    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
      txtPos.Text = $"X={Mouse.GetPosition(this).X}     Y={Mouse.GetPosition(this).Y}    POSICAODURA X={CompassoNovo.PosicaoDura().X}   Y={CompassoNovo.PosicaoDura().Y}";
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
      CompassoNovo.AdicionarPontoMarcacao();
    }

    Point central = new Point();
    public void KnobRotate_MouseDown(object sender, MouseButtonEventArgs e)
    {
      /*
       _____   ____ _______    _______ ______ 
      |  __ \ / __ \__   __|/\|__   __|  ____|
      | |__) | |  | | | |  /  \  | |  | |__   
      |  _  /| |  | | | | / /\ \ | |  |  __|  
      | | \ \| |__| | | |/ ____ \| |  | |____ 
      |_|  \_\\____/  |_/_/    \_\_|  |______|

      */
      // Calcula posição e tamanho **atualizada** do child em relação ao container
      NovaPosicao = CompassoAntigo.TransformToAncestor(this).TransformBounds(new Rect(CompassoAntigo.RenderSize));
      CompassoAntigo.RenderTransformOrigin = new Point(0, 1);
      isRotating = true;

      // Calcula o angulo inicial do giro
      double dx = e.GetPosition(ic).X - (NovaPosicao.X + (NovaPosicao.Width / 2));
      double dy = e.GetPosition(ic).Y - (NovaPosicao.Y + (NovaPosicao.Height / 2));
      StartAngle = Math.Atan2(dy, dx);

      // ******************* MULTIPLE *******************
      // No caso de multiplos elementos o ponto central é o adorner e não o centro do próprio elemento

      // Compensação do ponto central quando tem offset
      Matrix MatrixAtualB = ((MatrixTransform)RenderTransform).Matrix;
      double tmpOffsetX = MatrixAtualB.OffsetX;
      double tmpOffsetY = MatrixAtualB.OffsetY;

      // Calculo do ponto central para uso apenas no adorner
      central = MatrixAtualB.Transform(new Point(this.Width / 2, this.Height / 2));

      spc = new StylusPointCollection();
      Rect PosicaoPencil = CompassoAntigo.Pencil.TransformToAncestor(this).TransformBounds(new Rect(CompassoAntigo.Pencil.RenderSize));
      StylusPoint sp = new StylusPoint(PosicaoPencil.X, PosicaoPencil.Y);
      spc.Add(sp);
      ic.Strokes.Add(new Stroke(spc));
    }


  }
}
