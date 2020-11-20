using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_03_2_FinishNumerator
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_03_2_FinishNumerator : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Room> roomList = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfClass(typeof(SpatialElement))
                .Where(e => e.GetType() == typeof(Room))
                .Cast<Room>()
                .Where(r => r.Area > 0)
                .ToList();

            List<Floor> floorList = new FilteredElementCollector(doc)
               .OfClass(typeof(Floor))
               .Cast<Floor>()
               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол")
               .ToList();

            List<Wall> wallList = new FilteredElementCollector(doc)
               .OfClass(typeof(Wall))
               .Cast<Wall>()
               .Where(f => f.WallType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Отделка")
               .ToList();

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Заполнение описания");
                foreach (Floor floor in floorList)
                {
                    GeometryElement geomFloorElement = floor.get_Geometry(new Options());
                    Solid floorSolid = null;
                    foreach (GeometryObject geomObj in geomFloorElement)
                    {
                        floorSolid = geomObj as Solid;
                        if (floorSolid != null) break;
                    }
                    XYZ floorCenterPoint = floorSolid.ComputeCentroid();
                    Room room = doc
                        .GetRoomAtPoint(floorCenterPoint + (floor.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM)
                        .AsDouble()) * XYZ.BasisZ) as Room;

                    string floorDescription = floor.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION).AsString();
                    string roomNumber = room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();

                    if (floorDescription != null & floorDescription != "")
                    {
                        if (floorDescription.Contains(roomNumber))
                        {
                            continue;
                        }
                        else
                        {
                            floor.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION).Set(floorDescription + "," + roomNumber);
                        }
                    }
                    else
                    {
                        floor.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION).Set(roomNumber);
                    }
                }

                foreach (Wall wall in wallList)
                {
                    GeometryElement geomWallElement = wall.get_Geometry(new Options());
                    Solid wallSolid = null;
                    foreach (GeometryObject geomObj in geomWallElement)
                    {
                        wallSolid = geomObj as Solid;
                        if (wallSolid != null) break;
                    }
                    XYZ wallCenterPoint = wallSolid.ComputeCentroid();
                    XYZ wallOrientation = wall.Orientation;
                    Room room = doc
                        .GetRoomAtPoint(wallCenterPoint + wall.Width * wallOrientation) as Room;

                    string wallDescription = wall.WallType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION).AsString();
                    string roomNumber = room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();

                    if (wallDescription != null & wallDescription != "")
                    {
                        if (wallDescription.Contains(roomNumber))
                        {
                            continue;
                        }
                        else
                        {
                            wall.WallType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION).Set(wallDescription + "," + roomNumber);
                        }
                    }
                    else
                    {
                        wall.WallType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION).Set(roomNumber);
                    }
                }
                ElementClassFilter floorFilter = new ElementClassFilter(typeof(Floor));
                ElementClassFilter wallFilter = new ElementClassFilter(typeof(Wall));

                List<FloorType> floorTypeList = new FilteredElementCollector(doc)
                   .OfClass(typeof(FloorType))
                   .Where(ft => ft.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                   .Where(ft => ft.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол")
                   .Where(ft => ft.GetDependentElements(floorFilter).Count != 0)
                   .Cast<FloorType>()
                   .ToList();

                List<WallType> wallTypeList = new FilteredElementCollector(doc)
                   .OfClass(typeof(WallType))
                   .Where(wt => wt.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                   .Where(wt => wt.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Отделка")
                   .Where(wt => wt.GetDependentElements(wallFilter).Count != 0)
                   .Cast<WallType>()
                   .ToList();

                foreach (FloorType ft in floorTypeList)
                {
                    string ftDescr = ft.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION).AsString();
                    List<string> ftDescrList = ftDescr.Split(',').ToList();
                    List<int> ftDescrIntList = new List<int>();
                    foreach (string str in ftDescrList)
                    {
                        int strToInt = 0;
                        Int32.TryParse(str, out strToInt);
                        ftDescrIntList.Add(strToInt);
                    }

                    ftDescrIntList.Sort();
                    string newSortDescr = "";
                    foreach (int intToStr in ftDescrIntList)
                    {
                        if (newSortDescr == "")
                        {
                            newSortDescr = intToStr.ToString();
                        }
                        else
                        {
                            newSortDescr += ", " + intToStr.ToString();
                        }
                    }
                    ft.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION).Set(newSortDescr);
                }

                foreach (WallType wt in wallTypeList)
                {
                    string wtDescr = wt.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION).AsString();
                    List<string> wtDescrList = wtDescr.Split(',').ToList();
                    List<int> wtDescrIntList = new List<int>();
                    foreach (string str in wtDescrList)
                    {
                        int strToInt = 0;
                        Int32.TryParse(str, out strToInt);
                        wtDescrIntList.Add(strToInt);
                    }

                    wtDescrIntList.Sort();
                    string newSortDescr = "";
                    foreach (int intToStr in wtDescrIntList)
                    {
                        if (newSortDescr == "")
                        {
                            newSortDescr = intToStr.ToString();
                        }
                        else
                        {
                            newSortDescr += ", " + intToStr.ToString();
                        }
                    }
                    wt.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION).Set(newSortDescr);
                }

                t.Commit();
            }



            return Result.Succeeded;
        }
    }
}
