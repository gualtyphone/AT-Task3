using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using TerraMap.Data;

public class World {

    #region Dynamically read properties

    public Int32 Version { get; set; }
    public String Name { get; set; }

    public String Seed { get; set; }

    public ulong WorldGeneratorVersion { get; set; }

    public Guid UniqueId { get; set; }

    public Int32 Id { get; set; }
    public Rect Bounds { get; set; }
    public Int32 WorldHeightinTiles { get; set; }
    public Int32 WorldWidthinTiles { get; set; }

    public Boolean ExpertMode { get; set; }

    public Int64 CreationTime { get; set; }

    public Byte MoonType { get; set; }

    public Int32[] TreeTypeXCoordinates { get; set; } // x3

    public Int32[] TreeStyles { get; set; } // x4

    public Int32[] CaveBackXCoordinates { get; set; } // x3

    public Int32[] CaveBackStyles { get; set; } // x4

    public Int32 IceBackStyle { get; set; }
    public Int32 JungleBackStyle { get; set; }
    public Int32 HellBackStyle { get; set; }

    public Int32 SpawnX { get; set; }
    public Int32 SpawnY { get; set; }
    public Double WorldSurfaceY { get; set; }
    public Double RockLayerY { get; set; }
    public Double GameTime { get; set; }
    public Boolean IsDay { get; set; }
    public Int32 MoonPhase { get; set; }
    public Boolean BloodMoon { get; set; }

    public Boolean Eclipse { get; set; }

    public Int32 DungeonX { get; set; }
    public Int32 DungeonY { get; set; }

    public Boolean CrimsonWorld { get; set; }

    public Boolean KilledEyeofCthulu { get; set; }
    public Boolean KilledEaterofWorlds { get; set; }
    public Boolean KilledSkeletron { get; set; }

    public Boolean KilledQueenBee { get; set; }
    public Boolean KilledTheDestroyer { get; set; }
    public Boolean KilledTheTwins { get; set; }
    public Boolean KilledSkeletronPrime { get; set; }
    public Boolean KilledAnyHardmodeBoss { get; set; }
    public Boolean KilledPlantera { get; set; }
    public Boolean KilledGolem { get; set; }

    public Boolean KilledSlimeKing { get; set; }

    public Boolean SavedGoblinTinkerer { get; set; }
    public Boolean SavedWizard { get; set; }
    public Boolean SavedMechanic { get; set; }
    public Boolean DefeatedGoblinInvasion { get; set; }
    public Boolean KilledClown { get; set; }
    public Boolean DefeatedFrostLegion { get; set; }
    public Boolean DefeatedPirates { get; set; }

    public Boolean BrokeaShadowOrb { get; set; }
    public Boolean MeteorSpawned { get; set; }
    public Byte ShadowOrbsbrokenmod3 { get; set; }

    public Int32 AltarsSmashed { get; set; }
    public Boolean HardMode { get; set; }

    public Int32 GoblinInvasionDelay { get; set; }
    public Int32 GoblinInvasionSize { get; set; }
    public Int32 GoblinInvasionType { get; set; }
    public Double GoblinInvasionX { get; set; }

    public Double SlimeRainTime { get; set; }

    public Byte SundialCooldown { get; set; }

    public Boolean IsRaining { get; set; }
    public Int32 RainTime { get; set; }
    public Single MaxRain { get; set; }
    public Int32 Tier1OreID { get; set; }
    public Int32 Tier2OreID { get; set; }
    public Int32 Tier3OreID { get; set; }

    public Byte TreeStyle { get; set; }
    public Byte CorruptionStyle { get; set; }
    public Byte JungleStyle { get; set; }
    public Byte SnowStyle { get; set; }
    public Byte HallowStyle { get; set; }
    public Byte CrimsonStyle { get; set; }
    public Byte DesertStyle { get; set; }
    public Byte OceanStyle { get; set; }

    public Int32 CloudBackground { get; set; }
    public Int16 NumberofClouds { get; set; }
    public Single WindSpeed { get; set; }

    #endregion

    #region Other properties

    private string status;

    public string Status
    {
        get { return status; }
        set
        {
            status = value;
        }
    }

    public Tile[,] Tiles { get; set; }

    private int progressValue;

   
    public int ProgressValue
    {
        get { return progressValue; }
        set
        {
            progressValue = value;
        }
    }

    private int progressMaximum;

    public int ProgressMaximum
    {
        get { return progressMaximum; }
        set
        {
            progressMaximum = value;
        }
    }

    private int totalTileCount;

    public int TotalTileCount
    {
        get { return totalTileCount; }
        set
        {
            totalTileCount = value;
        }
    }

    private int highlightedTileCount;

    public int HighlightedTileCount
    {
        get { return highlightedTileCount; }
        set
        {
            highlightedTileCount = value;
        }
    }

    private bool invertHightlight;

    public bool InvertHighlight
    {
        get { return invertHightlight; }
        set
        {
            invertHightlight = value;
        }
    }

    public int HellLayerY { get; set; }

    #endregion


    public void Read(Stream s)
    {
        this.ProgressMaximum = 0;
        this.ProgressValue = 0;

        //this.OnProgressChanged(new ProgressEventArgs(tilesProcessed, totalTileCount));

        using (Stream stream = s)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                this.Version = reader.ReadInt32();

                reader.BaseStream.Position = 0L;


                this.ReadWorldVersion2(reader);
               
                //this.Status = "Reading Verification";
                //this.ReadVerification(reader);
            }
        }

        this.Status = string.Format("Finished reading");
    }

    private void ReadWorldVersion2(BinaryReader reader)
    {
        bool[] importance;
        int[] positions;

        this.LoadFileFormatHeader(reader, out importance, out positions);

        this.ReadHeader(reader, skipVersion: true);

        int hellLevel = ((this.WorldHeightinTiles - 230) - (int)this.WorldSurfaceY) / 6; //rounded
        hellLevel = hellLevel * 6 + (int)WorldSurfaceY - 5;
        this.HellLayerY = hellLevel;

        this.ReadTilesVersion2(reader, importance);

        return;

        //this.ReadChestsVersion2(reader);

        //this.Signs = new List<Sign>();

        //this.ReadSignsVersion2(reader);

        //this.ReadNPCsVersion2(reader);

        //this.ReadVerification(reader);
    }

    private void LoadFileFormatHeader(BinaryReader reader, out bool[] importance, out int[] positions)
    {
        this.Status = "Reading file format header...";

        this.Version = reader.ReadInt32();

        if (this.Version >= 135)
        {
            // read file metadata
            ulong num = reader.ReadUInt64();

            uint num3 = reader.ReadUInt32();

            ulong num2 = reader.ReadUInt64();
        }

        var positionsLength = reader.ReadInt16();
        positions = new int[(int)positionsLength];
        for (int i = 0; i < (int)positionsLength; i++)
        {
            positions[i] = reader.ReadInt32();
        }

        var importanceLength = reader.ReadInt16();
        importance = new bool[(int)importanceLength];
        byte b = 0;
        byte b2 = 128;
        for (int i = 0; i < (int)importanceLength; i++)
        {
            if (b2 == 128)
            {
                b = reader.ReadByte();
                b2 = 1;
            }
            else
            {
                b2 = (byte)(b2 << 1);
            }
            if ((b & b2) == b2)
            {
                importance[i] = true;
            }
        }
    }

    public void ReadHeader(BinaryReader reader, bool skipVersion = false)
    {
        this.Status = "Reading world header...";

        var properties = typeof(World).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.Name == "Version" && skipVersion)
            {
                continue;
            }

            var minimumVersion = 0;
            var count = 1;

            if (minimumVersion > this.Version)
                continue;

            var dataType = property.PropertyType;

            object value = null;

            if (dataType == typeof(Boolean))
                property.SetValue(this, reader.ReadBoolean(), null);
            else if (dataType == typeof(Byte))
                property.SetValue(this, reader.ReadByte(), null);
            else if (dataType == typeof(Int16))
                property.SetValue(this, reader.ReadInt16(), null);
            else if (dataType == typeof(Int32))
                property.SetValue(this, reader.ReadInt32(), null);
            else if (dataType == typeof(Int64))
                property.SetValue(this, reader.ReadInt64(), null);
            else if (dataType == typeof(String))
                property.SetValue(this, reader.ReadString(), null);
            else if (dataType == typeof(Single))
                property.SetValue(this, reader.ReadSingle(), null);
            else if (dataType == typeof(Double))
                property.SetValue(this, reader.ReadDouble(), null);
            else if (dataType == typeof(Rect))
                property.SetValue(this, ReadRectangle(reader), null);
            else if (dataType == typeof(Guid))
                property.SetValue(this, new Guid(reader.ReadBytes(16)), null);
            else if (dataType == typeof(ulong))
                property.SetValue(this, reader.ReadUInt64(), null);
            else if (dataType == typeof(Int32[]))
            {
                Int32[] array = new Int32[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = reader.ReadInt32();
                }

                if (count > 0)
                    //value = string.Join(", ", array);

                property.SetValue(this, array, null);
            }

            if (value == null)
                value = property.GetValue(this, null);

           // this.Properties.Add(new WorldProperty() { Name = property.Name, Value = value });
        }

        if (Version < 95)
            return;

        List<string> anglerWhoFinishedToday = new List<string>();

        for (int i = reader.ReadInt32(); i > 0; i--)
        {
            anglerWhoFinishedToday.Add(reader.ReadString());
        }

        if (Version < 99)
            return;

        reader.ReadBoolean();

        if (Version < 101)
            return;

        reader.ReadInt32();

        if (Version < 104)
            return;

        reader.ReadBoolean();

        if (Version >= 129)
        {
            reader.ReadBoolean();
        }

        if (Version < 107)
        {
        }
        else
        {
            reader.ReadInt32();
        }

        if (Version < 108)
        {
        }
        else
        {
            reader.ReadInt32();
        }

        if (Version < 109)
            return;

        int num2 = (int)reader.ReadInt16();
        for (int j = 0; j < num2; j++)
        {
            if (j < 541)
            {
                //this.NpcKillCount[j] = reader.ReadInt32();
                reader.ReadInt32();
            }
            else
            {
                reader.ReadInt32();
            }
        }

        if (Version < 128)
            return;

        reader.ReadBoolean()  ;
        if (Version < 131)
            return;

        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        if (Version < 140)
            return;

        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        if (Version < 170)
            return;

        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadInt32()  ;
        int num3 = reader.ReadInt32();
        for (int k = 0; k < num3; k++)
        {
            reader.ReadInt32();
        }

        if (Version < 174)
            return;

        reader.ReadBoolean()  ;
        reader.ReadInt32()  ;
        reader.ReadSingle()  ;
        reader.ReadSingle()  ;

        if (Version < 178)
            return;

        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
        reader.ReadBoolean()  ;
    }

    private void ReadTilesVersion2(BinaryReader reader, bool[] importance)
    {
        this.Status = "Reading tiles...";

        this.Tiles = new Tile[this.WorldWidthinTiles, this.WorldHeightinTiles];

        this.totalTileCount = this.WorldHeightinTiles * this.WorldWidthinTiles;

        var tilesProcessed = 0;

        this.ProgressMaximum = this.totalTileCount;
        this.ProgressValue = tilesProcessed;

        for (int x = 0; x < this.WorldWidthinTiles; x++)
        {
            for (int y = 0; y < this.WorldHeightinTiles; y++)
            {
                //if (x == 100 && y == 440)
                //	Debug.Write("");

                int num2 = -1;
                byte b2;
                byte b = b2 = 0;
                Tile tile = new Tile();
                byte b3 = reader.ReadByte();
                if ((b3 & 1) == 1)
                {
                    b2 = reader.ReadByte();
                    if ((b2 & 1) == 1)
                    {
                        b = reader.ReadByte();
                    }
                }
                byte b4;
                if ((b3 & 2) == 2)
                {
                    tile.IsActive = true;
                    if ((b3 & 32) == 32)
                    {
                        b4 = reader.ReadByte();
                        num2 = (int)reader.ReadByte();
                        num2 = (num2 << 8 | (int)b4);
                    }
                    else
                    {
                        num2 = (int)reader.ReadByte();
                    }

                    tile.Type = (ushort)num2;

                    //if (tile.Type > 254 && tile.Type != 280)
                    //	Debug.Write("");

                    if (importance[num2])
                    {
                        tile.TextureU = reader.ReadInt16();
                        tile.TextureV = reader.ReadInt16();
                        if (tile.Type == 144)
                        {
                            tile.TextureV = 0;
                        }
                        if (tile.Type == 26)
                        {
                        }
                    }
                    else
                    {
                        tile.TextureU = -1;
                        tile.TextureV = -1;

                        if (tile.Type == 105)
                        {
                        }
                    }
                    if ((b & 8) == 8)
                    {
                        tile.ColorValue = reader.ReadByte();
                    }
                }
                if ((b3 & 4) == 4)
                {
                    tile.WallType = reader.ReadByte();
                    tile.IsWallPresent = true;
                    if ((b & 16) == 16)
                    {
                        tile.WallColor = reader.ReadByte();
                        tile.IsWallColorPresent = true;
                    }
                }
                b4 = (byte)((b3 & 24) >> 3);
                if (b4 != 0)
                {
                    tile.IsLiquidPresent = true;
                    tile.LiquidAmount = reader.ReadByte();
                    if (b4 > 1)
                    {
                        if (b4 == 2)
                        {
                            tile.IsLiquidLava = true;
                        }
                        else
                        {
                            tile.IsLiquidHoney = true;
                        }
                    }
                }
                if (b2 > 1)
                {
                    if ((b2 & 2) == 2)
                    {
                        tile.IsRedWirePresent = true;
                    }
                    if ((b2 & 4) == 4)
                    {
                        tile.IsGreenWirePresent = true;
                    }
                    if ((b2 & 8) == 8)
                    {
                        tile.IsBlueWirePresent = true;
                    }
                    b4 = (byte)((b2 & 112) >> 4);
                    //if (b4 != 0 && this.SolidTiles[(int)tile.Type])
                    //{
                    //	if (b4 == 1)
                    //	{
                    //		tile.IsHalfTile = true;
                    //	}
                    //	else
                    //	{
                    //		tile.Slope = b4;
                    //	}
                    //}
                }
                if (b > 0)
                {
                    if ((b & 2) == 2)
                    {
                        tile.IsActuatorPresent = true;
                    }
                    if ((b & 4) == 4)
                    {
                        tile.IsActive = false;
                    }
                    if ((b & 32) == 32)
                    {
                        tile.IsYellowWirePresent = true;
                    }
                }
                b4 = (byte)((b3 & 192) >> 6);
                int k;
                if (b4 == 0)
                {
                    k = 0;
                }
                else
                {
                    if (b4 == 1)
                    {
                        k = (int)reader.ReadByte();
                    }
                    else
                    {
                        k = (int)reader.ReadInt16();
                    }
                }
                //if (num2 != -1)
                //{
                //	if ((double)y <= this.WorldSurfaceY)
                //	{
                //		if ((double)(y + k) <= this.worldSurface)
                //		{
                //			WorldGen.tileCounts[num2] += (k + 1) * 5;
                //		}
                //		else
                //		{
                //			int num3 = (int)(Main.worldSurface - (double)y + 1.0);
                //			int num4 = k + 1 - num3;
                //			WorldGen.tileCounts[num2] += num3 * 5 + num4;
                //		}
                //	}
                //	else
                //	{
                //		WorldGen.tileCounts[num2] += k + 1;
                //	}
                //}

                tile.Color = Color.black;

                this.Tiles[x, y] = tile;
                while (k > 0)
                {
                    y++;
                    this.Tiles[x, y] = tile;
                    var color = Color.black;
                    if (color != tile.Color)
                    {
                        var newTile = new Tile(tile);
                        newTile.Color = color;
                        this.Tiles[x, y] = newTile;
                    }
                    k--;
                }
            }

            tilesProcessed += this.WorldHeightinTiles;

            this.ProgressMaximum = this.totalTileCount;
            this.ProgressValue = tilesProcessed;

            this.Status = string.Format("Reading tile {0:N0} of {1:N0} ({2:P0})...", progressValue, progressMaximum, (float)progressValue / (float)progressMaximum);
        }
    }

    private static Rect ReadRectangle(BinaryReader reader)
    {
        var left = reader.ReadInt32();
        var right = reader.ReadInt32();
        var top = reader.ReadInt32();
        var bottom = reader.ReadInt32();

        return new Rect(left, top, right - left, bottom - top);
    }

}
