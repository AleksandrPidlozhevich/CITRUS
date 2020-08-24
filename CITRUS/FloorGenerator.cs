using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class FloorGenerator : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
			Document doc = commandData.Application.ActiveUIDocument.Document;
			Selection sel = commandData.Application.ActiveUIDocument.Selection;
			RoomSelectionFilter selFilter = new RoomSelectionFilter();
			IList<Reference> selRooms = sel.PickObjects(ObjectType.Element, selFilter, "Выберите помещения!");
			List<Room> roomList = new List<Room>();

			foreach (Reference roomRef in selRooms)
			{
				roomList.Add(doc.GetElement(roomRef) as Room);
			}

			View active = doc.ActiveView;
			Level myLevel = active.GenLevel;
			if(myLevel==null)
            {
				return Result.Failed;
			}

			List<FloorType> myFloorTypeList = new FilteredElementCollector(doc).OfClass(typeof(FloorType)).Cast<FloorType>().ToList();


		    FloorTypeSelector formRoomTypeSelector = new FloorTypeSelector(myFloorTypeList);
            formRoomTypeSelector.ShowDialog();
			FloorType myFloorType = formRoomTypeSelector.mySelectionFloorType;


			using (Transaction t = new Transaction(doc))
			{
				t.Start("Размещение пола");
				foreach (Room myRoom in roomList)
				{
					//string namRoom = myRoom.LookupParameter("Номер").AsString();

					CurveArray roomCurves = new CurveArray();
					IList<IList<BoundarySegment>> loops = myRoom.GetBoundarySegments(new SpatialElementBoundaryOptions());
					foreach (IList<BoundarySegment> loop in loops)
					{
						foreach (BoundarySegment seg in loop)
						{
							roomCurves.Append(seg.GetCurve());
						}
                        Floor myFloor2 = doc.Create.NewFloor(roomCurves, myFloorType, myLevel, false);
                        //Parameter kommentParam = myFloor.LookupParameter("Комментарии");
                        //kommentParam.Set(namRoom);
                    }
                }
				t.Commit();
			}


			return Result.Succeeded;
        }
    }
}
