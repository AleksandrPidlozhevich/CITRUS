using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_03_3_Insolation
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class InsolationAtPoint : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Доступ к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            //Вызов формы
            InsolationAtPointForm insolationAtPointForm = new InsolationAtPointForm();
            insolationAtPointForm.ShowDialog();
            if (insolationAtPointForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }
            bool checkSelectedPoints = insolationAtPointForm.CheckSelectedPoints;


            //Получение связанного файла "Инсоляционная линейка"
            IEnumerable<RevitLinkInstance> revitLinkInstanceInsolationRuler = new FilteredElementCollector(doc)
                    .OfClass(typeof(RevitLinkInstance))
                    .Where(li => li.Name.Contains("Инсоляционная линейка"))
                    .Cast<RevitLinkInstance>();

            if(revitLinkInstanceInsolationRuler.Count() == 0)
            {
                TaskDialog.Show("Revit", "Инсоляционная линейка не найдена!");
                return Result.Cancelled;
            }
            Document revitLinkInsolationRuler = revitLinkInstanceInsolationRuler.First().GetLinkDocument();

            //Получение лучей из файла "Инсоляционная линейка"
            List<ModelCurve> rayList = new FilteredElementCollector(revitLinkInsolationRuler)
                .OfCategory(BuiltInCategory.OST_Lines)
                .Cast<ModelCurve>()
                .ToList();
            if(rayList.Count() == 0)
            {
                TaskDialog.Show("Revit", "Инсоляционная линейка не содержит лучей");
                return Result.Cancelled;
            }

            //Получение векторов из лучей
            List<XYZ> rayVectorsList = new List<XYZ>();
            foreach(ModelCurve mc in rayList)
            {
                rayVectorsList.Add(((mc.Location as LocationCurve).Curve as Line).Direction.Normalize());
            }
            rayVectorsList.OrderBy(v => v.AngleTo(XYZ.BasisX));

            // Получение списка точек для проверки инсоляции
            List<FamilyInstance> insolationPointsList = new List<FamilyInstance>();
            if (checkSelectedPoints == false)
            {
                insolationPointsList = new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilyInstance))
                    .WhereElementIsNotElementType()
                    .Cast<FamilyInstance>()
                    .Where(fi => fi.Name == "InsolationPoint")
                    .ToList();
                if (insolationPointsList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Семейство \"InsolationPoint\" не размещено в проекте");
                    return Result.Cancelled;
                }
            }

            else
            {
                FamilyInstanceSelectionFilter selFilter = new FamilyInstanceSelectionFilter();
                IList<Reference> selFamilyInstances = sel.PickObjects(ObjectType.Element, selFilter, "Выберите помещения!");

                foreach (Reference genericModelsRef in selFamilyInstances)
                {
                    if ((doc.GetElement(genericModelsRef) as FamilyInstance).Name == "InsolationPoint")
                    {
                        insolationPointsList.Add(doc.GetElement(genericModelsRef) as FamilyInstance);
                    }
                }
            }



            //Получение списка твёрдых тел из Форм для проверки пересечений
            List<FamilyInstance> massList = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Mass)
                .OfClass(typeof(FamilyInstance))
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            List<Solid> solidList = new List<Solid>();
            foreach (FamilyInstance mass in massList)
            {
                GeometryElement geomElement = mass.get_Geometry(new Options());
                Solid solid = null;
                foreach (GeometryObject geomObj in geomElement)
                {
                    solid = geomObj as Solid;
                    if (solid != null) break;
                }
                solidList.Add(solid);
            }

            //Получение списка связанных файлов без "Инсоляционной линейки"
            List<RevitLinkInstance> revitLinkInstance = new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkInstance))
                .Where(li => !li.Name.Contains("Инсоляционная линейка"))
                .Cast<RevitLinkInstance>()
                .Where(li => li.GetLinkDocument() != null)
                .ToList();

            if (revitLinkInstance.Count() == 0)
            {
                TaskDialog.Show("Revit", "Связанные файлы не найдены");
                //return Result.Cancelled;
            }

            //Получение solid объектов из стен и перекрытий связанных файлов
            foreach (RevitLinkInstance rl in revitLinkInstance)
            {
                Transform transform = rl.GetTotalTransform();

                Document docWallFloor = rl.GetLinkDocument();
                //Получение солидов перекрытий из связанного файла
                List<Floor> floorList = new FilteredElementCollector(docWallFloor)
                    .WhereElementIsNotElementType()
                    .OfCategory(BuiltInCategory.OST_Floors)
                    .OfClass(typeof(Floor))
                    .Cast<Floor>()
                    .ToList();
                foreach(Floor floor in floorList)
                {
                    GeometryElement geomElement = floor.get_Geometry(new Options());
                    Solid solid = null;
                    foreach (GeometryObject geomObj in geomElement)
                    {
                        solid = geomObj as Solid;
                        if (solid != null) break;
                    }
                    Solid transformSolid = SolidUtils.CreateTransformed(solid, transform);
                    solidList.Add(transformSolid);
                }
                //Получение солидов стен из связанного файла
                List<Wall> wallList = new FilteredElementCollector(docWallFloor)
                    .WhereElementIsNotElementType()
                    .OfCategory(BuiltInCategory.OST_Walls)
                    .OfClass(typeof(Wall))
                    .Cast<Wall>()
                    .Where(w => w.CurtainGrid == null)
                    .ToList();
                foreach (Wall wall in wallList)
                {
                    GeometryElement geomElement = wall.get_Geometry(new Options());
                    Solid solid = null;
                    foreach (GeometryObject geomObj in geomElement)
                    {
                        solid = geomObj as Solid;
                        if (solid != null) break;
                    }
                    Solid transformSolid = SolidUtils.CreateTransformed(solid, transform);
                    solidList.Add(transformSolid);
                }
            }

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Расчет времени");
                SolidCurveIntersectionOptions intersectOptions = new SolidCurveIntersectionOptions();
                foreach (FamilyInstance p in insolationPointsList)
                {
                    int numberOfNonintersectingRays = 0;
                    int continuousInsolation = 0;
                    List<int> continuousInsolationList = new List<int>();

                    XYZ insolationPointLocation = (p.Location as LocationPoint).Point;
                    List<Curve> rayCurveListFromInsolationPoint = new List<Curve>();
                    foreach (XYZ v in rayVectorsList)
                    {
                        Curve curve = Line.CreateBound(insolationPointLocation, insolationPointLocation + (1000000 / 304.8) * v) as Curve;
                        rayCurveListFromInsolationPoint.Add(curve);
                    }

                    rayCurveListFromInsolationPoint.OrderBy(c => (c as Line).Direction.AngleTo(XYZ.BasisX));

                    for (int i=0; i< rayCurveListFromInsolationPoint.Count; i++)
                    {
                        int intersectionCount = 0;
                        foreach (Solid solid in solidList)
                        {
                            SolidCurveIntersection intersection = solid.IntersectWithCurve(rayCurveListFromInsolationPoint[i], intersectOptions);
                            var sc = intersection.SegmentCount;
                            if (sc > 0)
                            {
                                intersectionCount += 1;
                            }
                        }
                        if (intersectionCount == 0 & i != rayCurveListFromInsolationPoint.Count - 1)
                        {
                            numberOfNonintersectingRays += 1;
                            continuousInsolation += 1;
                        }
                        else if (intersectionCount == 0 & i == rayCurveListFromInsolationPoint.Count - 1)
                        {
                            numberOfNonintersectingRays += 1;
                            continuousInsolation += 1;
                            continuousInsolationList.Add(continuousInsolation);
                        }

                        else if (intersectionCount != 0 & i != rayCurveListFromInsolationPoint.Count - 1)
                        {
                            continuousInsolationList.Add(continuousInsolation);
                            continuousInsolation = 0;
                        }
                        else
                        {
                            continuousInsolationList.Add(continuousInsolation);
                        }
                    }

                    p.LookupParameter("Инсоляция (мин.)").Set(numberOfNonintersectingRays);
                    if (continuousInsolationList.Count != 0)
                    {
                        p.LookupParameter("Неприрывная инсоляция (мин.)").Set(continuousInsolationList.Max());
                        p.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Прерывистая");
                    }
                    else
                    {
                        p.LookupParameter("Неприрывная инсоляция (мин.)").Set(numberOfNonintersectingRays);
                        p.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Непрерывная");
                    }
                }
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
