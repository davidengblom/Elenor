namespace Elenor {
    public static class GenerationSeed {
        public static int ForFloor(int runSeed, int floorIndex) {
            unchecked { return runSeed * 73856093 ^ floorIndex * 19349663; }
        }

        public static int ForPrefabSelection(int floorSeed) {
            unchecked { return floorSeed * 83492791 ^ 0x50524546; } // "PREF
        }
    }
}