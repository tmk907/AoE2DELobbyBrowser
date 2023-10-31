﻿namespace AoE2DELobbyBrowser.WebApi
{
    public class MapTypeConverter
    {
        private const string Unknown = "Unknown";

        private static Dictionary<int, string> _map = new Dictionary<int, string>()
        {
            { 10875, "Arabia" },
            { 10876, "Archipelago" },
            { 10877, "Baltic" },
            { 10878, "BlackForest" },
            { 10879, "Coastal" },
            { 10880, "Continental" },
            { 10881, "CraterLake" },
            { 10882, "Fortress" },
            { 10883, "GoldRush" },
            { 10884, "Highland" },
            { 10885, "Islands" },
            { 10886, "Mediterranean" },
            { 10887, "Migration" },
            { 10888, "Rivers" },
            { 10889, "TeamIslands" },
            { 10891, "Scandanavia" },
            { 10892, "Mongolia" },
            { 10894, "Yucatan" },
            { 10893, "SaltMarsh" },
            { 10895, "Arena" },
            { 10897, "Oasis" },
            { 10898, "GhostLake" },
            { 10901, "Nomad" },
            { 10985, "Canals" },
            { 10986, "Capricious" },
            { 10987, "Dingos" },
            { 10988, "Graveyards" },
            { 10989, "Metropolis" },
            { 10946, "Moats" },
            { 10991, "ParadiseIsland" },
            { 10992, "Pilgrims" },
            { 10993, "Prairie" },
            { 10994, "Seasons" },
            { 10995, "SherwoodForest" },
            { 10996, "SherwoodHeroes" },
            { 10997, "Shipwreck" },
            { 10998, "TeamGlaciers" },
            { 10999, "TheUnknown" },
            { 13544, "RealWorldSpain" },
            { 13545, "RealWorldEngland" },
            { 13546, "RealWorldMideast" },
            { 13547, "RealWorldTexas" },
            { 13548, "RealWorldItaly" },
            { 13549, "RealWorldCaribbean" },
            { 13550, "RealWorldFrance" },
            { 13551, "RealWorldJutland" },
            { 13552, "RealWorldNippon" },
            { 13553, "RealWorldByzantium" },
            { 10914, "Acropolis" },
            { 10915, "Budapest" },
            { 10916, "Cenotes" },
            { 10917, "Cityoflakes" },
            { 10918, "Goldenpit" },
            { 10919, "Hideout" },
            { 10920, "Hillfort" },
            { 10921, "Lombardia" },
            { 10922, "Steppe" },
            { 10923, "Valley" },
            { 10924, "Megarandom" },
            { 10925, "Hamburger" },
            { 10926, "CtrRandom" },
            { 10927, "CtrMonsoon" },
            { 10928, "CtrPyramidDescent" },
            { 10929, "CtrSpiral" },
            { 301100, "Kilimanjaro" },
            { 301101, "MountainPass" },
            { 301102, "NileDelta" },
            { 301103, "Serengeti" },
            { 301104, "Socotra" },
            { 301105, "RealWorldAmazon" },
            { 301106, "RealWorldChina" },
            { 301107, "RealWorldHornOfAfrica" },
            { 301108, "RealWorldIndia" },
            { 301109, "RealWorldMadagascar" },
            { 301110, "RealWorldWestAfrica" },
            { 301111, "RealWorldBohemia" },
            { 301112, "RealWorldEarth" },
            { 301113, "SpecialMapCanyons" },
            { 301114, "SpecialMapArchipelago" },
            { 301115, "SpecialMapEnemyIslands" },
            { 301116, "SpecialMapFarOut" },
            { 301117, "SpecialMapFrontLine" },
            { 301118, "SpecialMapInnerCircle" },
            { 301119, "SpecialMapMotherland" },
            { 301120, "SpecialMapOpenPlains" },
            { 301121, "SpecialMapRingOfWater" },
            { 301122, "SpecialMapSnakePit" },
            { 301123, "SpecialMapTheEye" },
            { 301124, "RealWorldAustralia" },
            { 301125, "RealWorldIndochina" },
            { 301126, "RealWorldIndonesia" },
            { 301127, "RealWorldMalacca" },
            { 301128, "RealWorldPhilippines" },
            { 301129, "BogIslands" },
            { 301130, "MangroveJungle" },
            { 301131, "PacificIslands" },
            { 301132, "Sandbank" },
            { 301133, "WaterNomad" },
            { 301134, "SpecialMapJungleIslands" },
            { 301135, "SpecialMapHolyLine" },
            { 301136, "SpecialMapBorderStones" },
            { 301137, "SpecialMapYinYang" },
            { 301138, "SpecialMapJungleLanes" },
            { 301139, "AlpineLakes" },
            { 301141, "Bogland" },
            { 301142, "MountainRidge" },
            { 301143, "Ravines" },
            { 301144, "WolfHill" },
            { 301150, "SwirlingRiverSpecial" },
            { 301151, "TwinForestsSpecial" },
            { 301152, "JourneySouthSpecial" },
            { 301153, "SnakeForestSpecial" },
            { 301154, "SprawlingStreamsSpecial" },
            { 301145, "RealWorldAntarctica" },
            { 301146, "RealWorldAralSea" },
            { 301147, "RealWorldBlackSea" },
            { 301148, "RealWorldCaucasus" },
            { 301149, "RealWorldSiberia" },
            { 10930, "GoldenSwamp" },
            { 10931, "FourLakes" },
            { 10932, "LandNomad" },
            { 10933, "BattleOnTheIce" },
            { 10934, "ElDorado" },
            { 10935, "FallOfAxum" },
            { 10936, "FallOfRome" },
            { 10937, "TheMajapahitEmpire" },
            { 10938, "AmazonTunnel" },
            { 10939, "CoastalForest" },
            { 10940, "AfricanClearing" },
            { 10941, "Atacama" },
            { 10942, "SeizeTheMountain" },
            { 10943, "Crater" },
            { 10944, "Crossroads" },
            { 10945, "Michi" },
            { 10947, "VolcanicIsland" },
            { 10948, "Acclivity" },
            { 10949, "Eruption" },
            { 10950, "FrigidLake" },
            { 10951, "Greenland" },
            { 10952, "Lowland" },
            { 10953, "Marketplace" },
            { 10954, "Meadow" },
            { 10955, "MountainRange" },
            { 10956, "NorthernIsles" },
            { 10957, "RingFortress" },
            { 10958, "Runestones" },
            { 10959, "Aftermath" },
            { 10960, "Enclosed" },
            { 10961, "Haboob" },
            { 10962, "Kawasan" },
            { 10963, "LandMadness" },
            { 10964, "SacredSprings" },
            { 10965, "Wade" },
            { 10966, "Morass" },
            { 10967, "Shoals" },
        };

        public static string ToName(int? mapType)
        {
            if (mapType.HasValue)
            {
                if (_map.ContainsKey(mapType.Value)) return _map[mapType.Value];
            }
            return Unknown;
        }
    }
}
