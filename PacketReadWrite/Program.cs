// See https://aka.ms/new-console-template for more information
using MemoryPack;
using MessagePack;
using System.Text;


//
Console.WriteLine("BinaryReadWrite Test");
var binaryObj = new BinaryUserData();
binaryObj.UId = 71223323;
binaryObj.NickName = "mf78hbg";
binaryObj.ToConsole();

var binaryData = binaryObj.Seserialize();

var binaryObj2 = new BinaryUserData();
binaryObj2.Deserialize(binaryData);
binaryObj2.ToConsole();

Console.WriteLine("----------------------------------");
Console.WriteLine("");

//
Console.WriteLine("MessagePack Test");
var msgpackObj = new MessagePackUserData();
msgpackObj.UId = 32323;
msgpackObj.NickName = "dsdsdsdw2";
msgpackObj.ToConsole();

var msgpackData = MessagePackSerializer.Serialize(msgpackObj);
var msgpackObj2 = MessagePackSerializer.Deserialize<MessagePackUserData>(msgpackData);
msgpackObj2.ToConsole();

Console.WriteLine("----------------------------------");
Console.WriteLine("");

//
Console.WriteLine("MemoryPack Test");
var mempackObj = new MemoryPackUserData();
mempackObj.UId = 1122323;
mempackObj.NickName = "erwresdsdd";
mempackObj.ToConsole();

var mempackData = MemoryPackSerializer.Serialize(mempackObj);
var mempackOb2 = MemoryPackSerializer.Deserialize<MemoryPackUserData>(mempackData);
mempackOb2.ToConsole();

Console.WriteLine("----------------------------------");





