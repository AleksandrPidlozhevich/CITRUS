using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Architecture;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class FinishingSheet : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Room> roomsList = new FilteredElementCollector(doc)
                .OfClass(typeof(SpatialElement))
                .WhereElementIsNotElementType()
                .Where(room => room.GetType() == typeof(Room))
                .Cast<Room>()
                .ToList();

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Отделка");

                foreach (Room room in roomsList)
                {
                    double doorWindowArea = 0;

                    List<FamilyInstance> windowsListFromRoom = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Windows)
                    .WhereElementIsNotElementType()
                    .Cast<FamilyInstance>()
                    .Where(w => w.FromRoom != null)
                    .Where(w => w.FromRoom.Id == room.Id)
                    .ToList();
                    if (windowsListFromRoom.Count != 0)
                    {
                        foreach (FamilyInstance w in windowsListFromRoom)
                        {
                            doorWindowArea += w.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble() * w.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();
                        }
                    }

                    List<FamilyInstance> windowsListToRoom = new FilteredElementCollector(doc)
                   .OfCategory(BuiltInCategory.OST_Windows)
                   .WhereElementIsNotElementType()
                   .Cast<FamilyInstance>()
                   .Where(w => w.ToRoom != null)
                   .Where(w => w.ToRoom.Id == room.Id)
                   .ToList();
                    if (windowsListToRoom.Count != 0)
                    {
                        foreach (FamilyInstance w in windowsListToRoom)
                        {
                            doorWindowArea += w.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble() * w.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();
                        }
                    }

                    List<FamilyInstance> doorsListFromRoom = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .WhereElementIsNotElementType()
                    .Cast<FamilyInstance>()
                    .Where(d => d.FromRoom != null)
                    .Where(d => d.FromRoom.Id == room.Id)
                    .ToList();
                    if (doorsListFromRoom.Count != 0)
                    {
                        foreach (FamilyInstance d in doorsListFromRoom)
                        {
                            doorWindowArea += d.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble() * d.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();
                        }
                    }

                    List<FamilyInstance> doorsListToRoom = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .WhereElementIsNotElementType()
                    .Cast<FamilyInstance>()
                    .Where(d => d.ToRoom != null)
                    .Where(d => d.ToRoom.Id == room.Id)
                    .ToList();
                    if (doorsListToRoom.Count != 0)
                    {
                        foreach (FamilyInstance d in doorsListToRoom)
                        {
                            doorWindowArea += d.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble() * d.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();
                        }
                    }

                    room.LookupParameter("ADSK_Площадь проемов").Set(doorWindowArea);

                }


                List<string> descriptionFinishList = new List<string>();
                foreach (Room room in roomsList)
                {
                    string descriptionWallFinish = room.LookupParameter("Описание Стен").AsString();
                    string descriptionCeilingFinish = room.LookupParameter("Описание Потолка").AsString();
                    string descriptionFinish = descriptionWallFinish + "|" + descriptionCeilingFinish;
                    if (descriptionFinishList.Contains(descriptionFinish))
                    {
                        continue;
                    }
                    else
                    {
                        descriptionFinishList.Add(descriptionFinish);
                    }
                }

                foreach (string str in descriptionFinishList)
                {
                    string[] w = str.Split('|');

                    List<Room> roomsListNumeratorList = new FilteredElementCollector(doc)
                        .OfClass(typeof(SpatialElement))
                        .WhereElementIsNotElementType()
                        .Where(room => room.GetType() == typeof(Room))
                        .Where(room => room.LookupParameter("Описание Стен").AsString() == w[0])
                        .Where(room => room.LookupParameter("Описание Потолка").AsString() == w[1])
                        .Cast<Room>()
                        .ToList();

                    List<string> numbers = new List<string>();
                    foreach (Room r in roomsListNumeratorList)
                    {
                        numbers.Add(r.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
                    }
                    numbers.Sort(new AlphanumComparatorFastString());

                    string numbersStr = "";
                    foreach (string strNum in numbers)
                    {
                        int index = numbers.IndexOf(strNum);
                        if (index != numbers.Count - 1)
                        {
                            numbersStr = numbersStr + strNum + ", ";
                        }
                        else
                        {
                            numbersStr = numbersStr + strNum;
                        }
                    }

                    foreach (Room r in roomsListNumeratorList)
                    {
                        r.LookupParameter("Групповой номер-Стены").Set(numbersStr);
                    }
                }


                t.Commit();
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
