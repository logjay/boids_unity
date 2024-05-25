
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

    public bool Equals(RectBounds obj){
        if (obj.upper == upper &&
            obj.lower == lower &&
            obj.left == left &&
            obj.right == right){
            return true;
        }
        else{
            return false;
        }
    }
}