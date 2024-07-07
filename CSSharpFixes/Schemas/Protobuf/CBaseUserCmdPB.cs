namespace CSSharpFixes.Schemas.Protobuf;

public class CBaseUserCmdPB
{
    private IntPtr _address;
    private Dictionary<string, ulong> _offsets = new();
    
    public CBaseUserCmdPB(IntPtr address)
    {
        if (address == IntPtr.Zero) throw new ArgumentException("Address cannot be zero.");
        _address = address;
        
        _offsets.Add("SubtickMoves", 0x18);
        _offsets.Add("ForwardMove", 0x50);
        _offsets.Add("SideMove", 0x54);
        _offsets.Add("UpMove", 0x58);
        _offsets.Add("Impulse", 0x5C);
    }
    
    ~CBaseUserCmdPB()
    {
        _address = IntPtr.Zero;
        _offsets.Clear();
    }
    
    public Schemas.Protobuf.Interop.RepeatedPtrField<CSubTickMoveStep>? SubtickMoves
    {
        get
        {
            IntPtr subtickMovesPtr = (IntPtr)((ulong)_address + _offsets["SubtickMoves"]);
            if(subtickMovesPtr == IntPtr.Zero) return null;
            return new Schemas.Protobuf.Interop.RepeatedPtrField<CSubTickMoveStep>(subtickMovesPtr);
        }
    }
    
    public float? ForwardMove
    {
        get
        {
            IntPtr forwardMovePtr = (IntPtr)((ulong)_address + _offsets["ForwardMove"]);
            if(forwardMovePtr == IntPtr.Zero) return null;

            unsafe
            {
                float* forwardMove = (float*)forwardMovePtr.ToPointer();
                return *forwardMove;
            }
        }
    }
    
    public float? SideMove
    {
        get
        {
            IntPtr sideMovePtr = (IntPtr)((ulong)_address + _offsets["SideMove"]);
            if(sideMovePtr == IntPtr.Zero) return null;

            unsafe
            {
                float* sideMove = (float*)sideMovePtr.ToPointer();
                return *sideMove;
            }
        }
    }
    
    public float? UpMove
    {
        get
        {
            IntPtr upMovePtr = (IntPtr)((ulong)_address + _offsets["UpMove"]);
            if(upMovePtr == IntPtr.Zero) return null;

            unsafe
            {
                float* upMove = (float*)upMovePtr.ToPointer();
                return *upMove;
            }
        }
    }
    
    public int? Impulse
    {
        get
        {
            IntPtr impulsePtr = (IntPtr)((ulong)_address + _offsets["Impulse"]);
            if(impulsePtr == IntPtr.Zero) return null;

            unsafe
            {
                int* impulse = (int*)impulsePtr.ToPointer();
                return *impulse;
            }
        }
    }
}