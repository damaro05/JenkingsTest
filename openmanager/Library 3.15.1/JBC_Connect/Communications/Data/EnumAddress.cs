namespace JBC_Connect
{
    public enum EnumAddress : byte
    {
        //Masks
        MASK_STATION_ADDRESS = 0x70,
        MASK_BROADCAST_ADDRESS = 0xF,
        MASK_RESPONSE_ADDRESS = 0x80,

        //SubStations
        NEXT_SUBSTATION_ADDRESS = 0x10,

        //Special address
        CONTINUOUS_MODE_DSPIC33_ADDRESS = 0x11
    }
}
