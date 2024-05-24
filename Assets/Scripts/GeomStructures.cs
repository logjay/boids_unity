
public struct RectBounds{
    public float upper;
    public float lower;
    public float left;
    public float right;

    public RectBounds(int size=100){
        upper = size;
        lower = -size;
        left = -size;
        right = size;
    }
}