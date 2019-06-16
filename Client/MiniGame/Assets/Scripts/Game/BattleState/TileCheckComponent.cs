public class TileCheckComponent : DataComponent
{
    public byte[,] stateCaches = null;  //操作缓存
}

public class TileCheckState
{
    public static byte None = 0;
    public static byte WaitCheck = 1 << 0;
    public static byte ColumnChecked = 1 << 1;
    public static byte RowChecked = 1 << 2;
    public static byte AllChecked = (byte)(ColumnChecked + RowChecked);
    public static byte CheckedSuccess = 1 << 3;
}