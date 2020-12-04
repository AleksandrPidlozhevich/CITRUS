using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

            List<Room> roomList = new FilteredElementCollector(doc)
                .OfClass(typeof(SpatialElement))
                .WhereElementIsNotElementType()
                .Where(r => r.GetType() == typeof(Room))
                .Cast<Room>()
                .ToList();

            if (divideByFloors == false)
            {
                List<Floor> floorList = new FilteredElementCollector(doc)
                       .OfClass(typeof(Floor))
                       .Cast<Floor>()
                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол" 
                       || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                       .ToList();

                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Заполнение номеров помещений");
                    //Очистка параметра "Помещение_Список номеров"
                    foreach (Floor floor in floorList)
                    {
                        floor.LookupParameter("Помещение_Список номеров").Set("");
                    }

                    foreach (Floor floor in floorList)
                    {
                        Room room = null;
                        Floor floorForSolid = doc.GetElement(ElementTransformUtils.CopyElement(doc, floor.Id, (100 / 304.8) * XYZ.BasisZ).First()) as Floor;
                        GeometryElement geomFloorElement = floorForSolid.get_Geometry(new Options());
                        Solid floorSolid = null;
                        foreach (GeometryObject geomObj in geomFloorElement)
                        {
                            floorSolid = geomObj as Solid;
                            if (floorSolid != null) break;
                        }

                        foreach (Room r in roomList)
                        {
                            GeometryElement geomRoomElement = r.get_Geometry(new Options());
                            Solid roomSolid = null;
                            foreach (GeometryObject geomObj in geomRoomElement)
                            {
                                roomSolid = geomObj as Solid;
                                if (roomSolid != null) break;
                            }
                            Solid intersection = null;
                            try
                            {
                                intersection = BooleanOperationsUtils.ExecuteBooleanOperation(floorSolid, roomSolid, BooleanOperationsType.Intersect);
                            }
                            catch
                            {
                                TaskDialog.Show("Revit", "Не удалось обработать "
                                    + floor.FloorType.Name + "\nи помещение №" + r.Number.ToString()
                                    + " из за ошибок геометрии");
                            }
                            double volumeOfIntersection = 0;
                            if(intersection != null)
                            {
                                volumeOfIntersection = intersection.Volume;
                            }
                            if (volumeOfIntersection != 0)
                            {
                                room = r;
                                break;
                            }
                        }
                        doc.Delete(floorForSolid.Id);

                        if (room != null)
                        {
                            string floorDescription = floor.LookupParameter("Помещение_Список номеров").AsString();
                            string roomNumber = room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();
                            if (floorDescription != null & floorDescription != "")
                            {
                                if (floorDescription.Split(',').ToList().Contains(roomNumber))
                                {
                                    continue;
                                }
                                else
                                {
                                    List<Floor> floorListForFilling = new FilteredElementCollector(doc)
                                       .OfClass(typeof(Floor))
                                       .Cast<Floor>()
                                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол" 
                                       || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
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
                                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                                       || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
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
                   .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                   || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
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
                                floorRoomsNumbersList.Sort(new AlphanumComparatorFastString());
                                List<string> floorRoomsNumbersSortedList = floorRoomsNumbersList;
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
                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                               || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
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
                           .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                           || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                           .Where(f => f.LevelId == lv.Id)
                           .ToList();

                        if (floorList.Count != 0)
                        {
                            //Очистка параметра "Помещение_Список номеров"
                            foreach (Floor floor in floorList)
                            {
                                floor.LookupParameter("Помещение_Список номеров").Set("");
                            }

                            foreach (Floor floor in floorList)
                            {
                                Room room = null;
                                Floor floorForSolid = doc.GetElement(ElementTransformUtils.CopyElement(doc, floor.Id, (100 / 304.8) * XYZ.BasisZ).First()) as Floor;
                                GeometryElement geomFloorElement = floorForSolid.get_Geometry(new Options());
                                Solid floorSolid = null;
                                foreach (GeometryObject geomObj in geomFloorElement)
                                {
                                    floorSolid = geomObj as Solid;
                                    if (floorSolid != null) break;
                                }

                                foreach (Room r in roomList)
                                {
                                    GeometryElement geomRoomElement = r.get_Geometry(new Options());
                                    Solid roomSolid = null;
                                    foreach (GeometryObject geomObj in geomRoomElement)
                                    {
                                        roomSolid = geomObj as Solid;
                                        if (roomSolid != null) break;
                                    }
                                    Solid intersection = null;
                                    try
                                    {
                                        intersection = BooleanOperationsUtils.ExecuteBooleanOperation(floorSolid, roomSolid, BooleanOperationsType.Intersect);
                                    }
                                    catch
                                    {
                                        TaskDialog.Show("Revit", "Не удалось обработать "
                                            + floor.FloorType.Name + "\nи помещение №" + r.Number.ToString()
                                            + " из за ошибок геометрии");
                                    }
                                    double volumeOfIntersection = 0;
                                    if (intersection != null)
                                    {
                                        volumeOfIntersection = intersection.Volume;
                                    }
                                    if (volumeOfIntersection != 0)
                                    {
                                        room = r;
                                        break;
                                    }
                                }
                                doc.Delete(floorForSolid.Id);

                                if (room != null)
                                {
                                    string floorDescription = floor.LookupParameter("Помещение_Список номеров").AsString();
                                    string roomNumber = room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();
                                    if (floorDescription != null & floorDescription != "")
                                    {
                                        if (floorDescription.Split(',').ToList().Contains(roomNumber))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            List<Floor> floorListForFilling = new FilteredElementCollector(doc)
                                               .OfClass(typeof(Floor))
                                               .Cast<Floor>()
                                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                                               || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
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
                                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                                               || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
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
                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                               || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
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
                                        floorRoomsNumbersList.Sort(new AlphanumComparatorFastString());
                                        List<string> floorRoomsNumbersSortedList = floorRoomsNumbersList;
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
                                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                                       || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
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

    public class AlphanumComparatorFastString : IComparer<String>
    {
        public int Compare(string s1, string s2)
        {
            if (s1 == null)
                return 0;

            if (s2 == null)
                return 0;

            int len1 = s1.Length;
            int len2 = s2.Length;
            int marker1 = 0;
            int marker2 = 0;

            // Walk through two the strings with two markers.
            while (marker1 < len1 && marker2 < len2)
            {
                char ch1 = s1[marker1];
                char ch2 = s2[marker2];

                // Some buffers we can build up characters in for each chunk.
                char[] space1 = new char[len1];
                int loc1 = 0;
                char[] space2 = new char[len2];
                int loc2 = 0;

                // Walk through all following characters that are digits or
                // characters in BOTH strings starting at the appropriate marker.
                // Collect char arrays.
                do
                {
                    space1[loc1++] = ch1;
                    marker1++;

                    if (marker1 < len1)
                    {
                        ch1 = s1[marker1];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

                do
                {
                    space2[loc2++] = ch2;
                    marker2++;

                    if (marker2 < len2)
                    {
                        ch2 = s2[marker2];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

                // If we have collected numbers, compare them numerically.
                // Otherwise, if we have strings, compare them alphabetically.
                string str1 = new string(space1);
                string str2 = new string(space2);

                int result;

                if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
                {
                    int thisNumericChunk = int.Parse(str1);
                    int thatNumericChunk = int.Parse(str2);
                    result = thisNumericChunk.CompareTo(thatNumericChunk);
                }
                else
                {
                    result = str1.CompareTo(str2);
                }

                if (result != 0)
                {
                    return result;
                }
            }
            return len1 - len2;
        }
    }
}
