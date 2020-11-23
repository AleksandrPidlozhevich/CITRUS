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

            CIT_03_2_FinishNumeratorForm finishNumeratorForm = new CIT_03_2_FinishNumeratorForm();
            finishNumeratorForm.ShowDialog();
            if (finishNumeratorForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }
            bool divideByFloors = finishNumeratorForm.DivideByFloors;

            if (divideByFloors == false)
            {
                List<Floor> floorList = new FilteredElementCollector(doc)
                       .OfClass(typeof(Floor))
                       .Cast<Floor>()
                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                       .ToList();

                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Заполнение номеров помещений");
                    //Очистка параметра "Помещение_Список номеров"
                    foreach (Floor f in floorList)
                    {
                        f.LookupParameter("Помещение_Список номеров").Set("");
                    }

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
                        if (room != null)
                        {
                            string floorDescription = floor.LookupParameter("Помещение_Список номеров").AsString();
                            string roomNumber = room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();
                            if (floorDescription != null & floorDescription != "")
                            {
                                if (floorDescription.Contains(roomNumber))
                                {
                                    continue;
                                }
                                else
                                {
                                    List<Floor> floorListForFilling = new FilteredElementCollector(doc)
                                       .OfClass(typeof(Floor))
                                       .Cast<Floor>()
                                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                                       .Where(f => f.FloorType.Id == floor.FloorType.Id)
                                       .ToList();
                                    foreach (Floor f in floorListForFilling)
                                    {
                                        f.LookupParameter("Помещение_Список номеров").Set(floorDescription + "," + roomNumber);
                                    }
                                }
                            }
                            else
                            {
                                List<Floor> floorListForFilling = new FilteredElementCollector(doc)
                                       .OfClass(typeof(Floor))
                                       .Cast<Floor>()
                                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                                       .Where(f => f.FloorType.Id == floor.FloorType.Id)
                                       .ToList();
                                foreach (Floor f in floorListForFilling)
                                {
                                    f.LookupParameter("Помещение_Список номеров").Set(roomNumber);
                                }
                            }
                        }
                    }

                    List<Floor> floorListForSortedFilling = new FilteredElementCollector(doc)
                   .OfClass(typeof(Floor))
                   .Cast<Floor>()
                   .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                   .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                   .ToList();

                    List<ElementId> FloorTypeIdList = new List<ElementId>();

                    foreach (Floor floor in floorListForSortedFilling)
                    {
                        if (FloorTypeIdList.Contains(floor.FloorType.Id))
                        {
                            continue;
                        }
                        else
                        {
                            FloorTypeIdList.Add(floor.FloorType.Id);
                            string newfloorRoomsNumbersSorted = "";
                            string floorRoomsNumbers = floor.LookupParameter("Помещение_Список номеров").AsString();
                            if (floorRoomsNumbers != null & floorRoomsNumbers != "")
                            {
                                List<string> floorRoomsNumbersList = floorRoomsNumbers.Split(',').ToList();
                                List<string> floorRoomsNumbersSortedList = floorRoomsNumbersList.OrderBy(x => x).ToList();
                                foreach (string st in floorRoomsNumbersSortedList)
                                {
                                    if (newfloorRoomsNumbersSorted == "")
                                    {
                                        newfloorRoomsNumbersSorted = st;
                                    }
                                    else
                                    {
                                        newfloorRoomsNumbersSorted += ", " + st;
                                    }
                                }


                            }
                            List<Floor> floorListForAddNewSortedFilling = new FilteredElementCollector(doc)
                               .OfClass(typeof(Floor))
                               .Cast<Floor>()
                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                               .Where(f => f.FloorType.Id == floor.FloorType.Id)
                               .ToList();
                            foreach (Floor f in floorListForAddNewSortedFilling)
                            {
                                f.LookupParameter("Помещение_Список номеров").Set(newfloorRoomsNumbersSorted);
                            }
                        }
                    }
                    t.Commit();
                }
            }

            if (divideByFloors == true)
            {
                List<Level> levelList = new FilteredElementCollector(doc)
                       .OfClass(typeof(Level))
                       .Cast<Level>()
                       .ToList();

                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Заполнение номеров помещений");
                    foreach (Level lv in levelList)
                    {

                        List<Floor> floorList = new FilteredElementCollector(doc)
                           .OfClass(typeof(Floor))
                           .Cast<Floor>()
                           .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                           .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                           .Where(f => f.LevelId == lv.Id)
                           .ToList();

                        if (floorList.Count != 0)
                        {
                            //Очистка параметра "Помещение_Список номеров"
                            foreach (Floor f in floorList)
                            {
                                f.LookupParameter("Помещение_Список номеров").Set("");
                            }

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
                                if (room != null)
                                {
                                    string floorDescription = floor.LookupParameter("Помещение_Список номеров").AsString();
                                    string roomNumber = room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();
                                    if (floorDescription != null & floorDescription != "")
                                    {
                                        if (floorDescription.Contains(roomNumber))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            List<Floor> floorListForFilling = new FilteredElementCollector(doc)
                                               .OfClass(typeof(Floor))
                                               .Cast<Floor>()
                                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                                               .Where(f => f.FloorType.Id == floor.FloorType.Id)
                                               .Where(f => f.LevelId == lv.Id)
                                               .ToList();
                                            foreach (Floor f in floorListForFilling)
                                            {
                                                f.LookupParameter("Помещение_Список номеров").Set(floorDescription + "," + roomNumber);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        List<Floor> floorListForFilling = new FilteredElementCollector(doc)
                                               .OfClass(typeof(Floor))
                                               .Cast<Floor>()
                                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                                               .Where(f => f.FloorType.Id == floor.FloorType.Id)
                                               .Where(f => f.LevelId == lv.Id)
                                               .ToList();
                                        foreach (Floor f in floorListForFilling)
                                        {
                                            f.LookupParameter("Помещение_Список номеров").Set(roomNumber);
                                        }
                                    }
                                }
                            }

                            List<Floor> floorListForSortedFilling = new FilteredElementCollector(doc)
                               .OfClass(typeof(Floor))
                               .Cast<Floor>()
                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                               .Where(f => f.LevelId == lv.Id)
                               .ToList();

                            List<ElementId> FloorTypeIdList = new List<ElementId>();

                            foreach (Floor floor in floorListForSortedFilling)
                            {
                                if (FloorTypeIdList.Contains(floor.FloorType.Id))
                                {
                                    continue;
                                }
                                else
                                {
                                    FloorTypeIdList.Add(floor.FloorType.Id);
                                    string newfloorRoomsNumbersSorted = "";
                                    string floorRoomsNumbers = floor.LookupParameter("Помещение_Список номеров").AsString();
                                    if (floorRoomsNumbers != null & floorRoomsNumbers != "")
                                    {
                                        List<string> floorRoomsNumbersList = floorRoomsNumbers.Split(',').ToList();
                                        List<string> floorRoomsNumbersSortedList = floorRoomsNumbersList.OrderBy(x => x).ToList();
                                        foreach (string st in floorRoomsNumbersSortedList)
                                        {
                                            if (newfloorRoomsNumbersSorted == "")
                                            {
                                                newfloorRoomsNumbersSorted = st;
                                            }
                                            else
                                            {
                                                newfloorRoomsNumbersSorted += ", " + st;
                                            }
                                        }


                                    }
                                    List<Floor> floorListForAddNewSortedFilling = new FilteredElementCollector(doc)
                                       .OfClass(typeof(Floor))
                                       .Cast<Floor>()
                                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                                       .Where(f => f.FloorType.Id == floor.FloorType.Id)
                                       .Where(f => f.LevelId == lv.Id)
                                       .ToList();
                                    foreach (Floor f in floorListForAddNewSortedFilling)
                                    {
                                        f.LookupParameter("Помещение_Список номеров").Set(newfloorRoomsNumbersSorted);
                                    }
                                }
                            }
                        }
                    }
                    t.Commit();
                }
            }
            return Result.Succeeded;
        }
    }
}
