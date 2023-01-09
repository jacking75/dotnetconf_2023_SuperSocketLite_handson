using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOServer;

class RoomManager
{
    List<Room> RoomsList = new List<Room>();

    public void CreateRooms()
    {
        var maxRoomCount = MainServer.Conf_RoomMaxCount;
        var maxUserCount = MainServer.Conf_RoomMaxUserCount;

        for(int i = 0; i < maxRoomCount; ++i)
        {
            var roomNumber = i;
            var room = new Room();
            room.Init(i, roomNumber, maxUserCount);

            RoomsList.Add(room);
        }                                   
    }


    public List<Room> GetRoomsList() { return RoomsList; }
}
