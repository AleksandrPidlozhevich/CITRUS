using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Structure;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class GloryHole : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

			//Список всех стен
			List<Element> walllsList = new FilteredElementCollector(doc)
				.OfCategory(BuiltInCategory.OST_Walls)
				.WhereElementIsNotElementType()
				.ToList();

			//Список семейств с именем IntersectionPoint
			List<Family> families = new FilteredElementCollector(doc).OfClass(typeof(Family)).Cast<Family>().Where(f => f.Name == "IntersectionPoint").ToList();
			if (families.Count != 1) return Result.Failed;
			Family mainFam = families.First();

			//ID элемента IntersectionPoint
			List<ElementId> symbolsIds = mainFam.GetFamilySymbolIds().ToList();
			ElementId firstSymbolId = symbolsIds.First();

			//Тип элемента(FamilySymbol) IntersectionPoint
			FamilySymbol mySymbol = doc.GetElement(firstSymbolId) as FamilySymbol;
			if (mySymbol == null) return Result.Failed;

			//Точка вставки IntersectionPoint
			XYZ centerPoint = new XYZ(0, 0, 0);
			using (Transaction t = new Transaction(doc))
			{
				t.Start("Размещение семейств");

				foreach (Wall wall in walllsList)
				{
					//Получение ширины стены
					double width = wall.Width;
					//Получение уровня стены;
					Level myLevel = doc.GetElement(wall.LevelId) as Level;
					//Получение коэффициентов уравнения прямой
					LocationCurve wallLocationCurve = wall.Location as LocationCurve;
					Curve wallCurve = wallLocationCurve.Curve; //Получение кривой эскиза стены
					XYZ endWall0 = wallCurve.GetEndPoint(0); //Получение начальной точки кривой
					XYZ endWall1 = wallCurve.GetEndPoint(1); //Получение конечной точки кривой
					//Получение отдельных координат точек
					double x0 = endWall0.X; 
					double y0 = endWall0.Y; 
					double x1 = endWall1.X;
					double y1 = endWall1.Y;
					//Получение коэффициентов уравнения прямой
					double A1 = y0 - y1;
					double B1 = x1 - x0;

					Solid wallSolid = null;
					GeometryElement wallGeometryElement = wall.get_Geometry(new Options());
					foreach (GeometryObject geoObject in wallGeometryElement)
					{
						wallSolid = geoObject as Solid;
						if (wallSolid != null) break;
					}
					List<Element> pipesListIntersect = new FilteredElementCollector(doc)
						.OfCategory(BuiltInCategory.OST_PipeCurves)
						.WherePasses(new ElementIntersectsSolidFilter(wallSolid))
						.ToList();

					List<Element> ductsListIntersect = new FilteredElementCollector(doc)
						.OfCategory(BuiltInCategory.OST_DuctCurves)
						.WherePasses(new ElementIntersectsSolidFilter(wallSolid))
						.ToList();

					foreach (Element pipe in pipesListIntersect)
					{

						//Получение коэффициентов уравнения прямой(аналогично стене)
						LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
						Curve pipeCurve = pipeLocationCurve.Curve;
						Curve line = wallSolid.IntersectWithCurve(pipeCurve, new SolidCurveIntersectionOptions()).GetCurveSegment(0);
						XYZ end0 = line.GetEndPoint(0);
						XYZ end1 = line.GetEndPoint(1);
						double x0_ = end0.X;
						double y0_ = end0.Y;
						double x1_ = end1.X;
						double y1_ = end1.Y;
						double z0 = end0.Z;
						double z1 = end1.Z;
						double A2 = y0_ - y1;
						double B2 = x1_ - x0;
						double C2 = z1 - z0;
						//Получение центра пересечения стены и трубы
						centerPoint = new XYZ((end0.X + end1.X) / 2, (end0.Y + end1.Y) / 2, (end0.Z + end1.Z) / 2); //Получение центра пересечения стены и трубы
						FamilyInstance fi = doc.Create.NewFamilyInstance(centerPoint, mySymbol, myLevel, StructuralType.NonStructural);

					}

					foreach (Element duct in ductsListIntersect)
					{
						//Получение коэффициентов уравнения прямой(аналогично стене)
						LocationCurve ductLocationCurve = duct.Location as LocationCurve;
						Curve ductCurve = ductLocationCurve.Curve;
						Curve line = wallSolid.IntersectWithCurve(ductCurve, new SolidCurveIntersectionOptions()).GetCurveSegment(0);
						XYZ end0 = line.GetEndPoint(0);
						XYZ end1 = line.GetEndPoint(1);
						double x0_ = end0.X;
						double y0_ = end0.Y;
						double x1_ = end1.X;
						double y1_ = end1.Y;
						double z0 = end0.Z;
						double z1 = end1.Z;
						double A2 = y0_ - y1;
						double B2 = x1_ - x0;
						double C2 = z1 - z0;
						//Получение центра пересечения стены и трубы
						centerPoint = new XYZ((end0.X + end1.X) / 2, (end0.Y + end1.Y) / 2, (end0.Z + end1.Z) / 2); //Получение центра пересечения стены и трубы
						FamilyInstance fi = doc.Create.NewFamilyInstance(centerPoint, mySymbol, myLevel, StructuralType.NonStructural);
					}
				}
				t.Commit();
			}
			return Result.Succeeded;
        }
    }
}
