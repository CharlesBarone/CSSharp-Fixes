namespace CSSharpFixes.Schemas.Protobuf;

public class CBaseUserCmdPB: Interfaces.ISizeable
{
    public static ulong Size() => 0x80;
    
    private IntPtr _address;
    private Dictionary<string, ulong> _offsets = new();
    
    public CBaseUserCmdPB(IntPtr address)
    {
        if (address == IntPtr.Zero) throw new ArgumentException("Address cannot be zero.");
        _address = address;
        
        // CBasePB size = 0x18
        _offsets.Add("SubtickMoves", 0x18); // RepeatedPtrField<CSubTickMoveStep>
        // std::string* strMoveCrc; offset = 0x30. size = 0x8
        // CInButtonStatePB* pInButtonState; offset = 0x38. size = 0x8
        // CMsgQAngle* pViewAngles; offset = 0x40. size = 0x8
        _offsets.Add("CommandNumber", 0x48); // int
        _offsets.Add("ClientTick", 0x4C); // int
        _offsets.Add("ForwardMove", 0x50); // float
        _offsets.Add("SideMove", 0x54); // float
        _offsets.Add("UpMove", 0x58); // float
        _offsets.Add("Impulse", 0x5C); // int
        _offsets.Add("WeaponSelect", 0x60); // int
        _offsets.Add("RandomSeed", 0x64); // int
        _offsets.Add("MouseX", 0x68); // int
        _offsets.Add("MouseY", 0x6C); // int
        _offsets.Add("ConsumedServerAngleChanges", 0x70); // uint
        _offsets.Add("CmdFlags", 0x74); // int
        _offsets.Add("PawnEntityHandle", 0x78); // uint
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
    
    public int? ClientTick
    {
        get
        {
            IntPtr clientTickPtr = (IntPtr)((ulong)_address + _offsets["ClientTick"]);
            if(clientTickPtr == IntPtr.Zero) return null;

            unsafe
            {
                int* clientPtr = (int*)clientTickPtr.ToPointer();
                return *clientPtr;
            }
        }
    }
    
    public int? CommandNumber
    {
        get
        {
            IntPtr commandNumberPtr = (IntPtr)((ulong)_address + _offsets["CommandNumber"]);
            if(commandNumberPtr == IntPtr.Zero) return null;

            unsafe
            {
                int* commandNumber = (int*)commandNumberPtr.ToPointer();
                return *commandNumber;
            }
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
    
    public int? WeaponSelect
    {
        get
        {
            IntPtr weaponSelectPtr = (IntPtr)((ulong)_address + _offsets["WeaponSelect"]);
            if(weaponSelectPtr == IntPtr.Zero) return null;

            unsafe
            {
                int* weaponSelect = (int*)weaponSelectPtr.ToPointer();
                return *weaponSelect;
            }
        }
    }
    
    public int? RandomSeed
    {
        get
        {
            IntPtr randomSeedPtr = (IntPtr)((ulong)_address + _offsets["RandomSeed"]);
            if(randomSeedPtr == IntPtr.Zero) return null;

            unsafe
            {
                int* randomSeed = (int*)randomSeedPtr.ToPointer();
                return *randomSeed;
            }
        }
    }
    
    public int? MouseX
    {
        get
        {
            IntPtr mouseXPtr = (IntPtr)((ulong)_address + _offsets["MouseX"]);
            if(mouseXPtr == IntPtr.Zero) return null;

            unsafe
            {
                int* mouseX = (int*)mouseXPtr.ToPointer();
                return *mouseX;
            }
        }
    }
    
    public int? MouseY
    {
        get
        {
            IntPtr mouseYPtr = (IntPtr)((ulong)_address + _offsets["MouseY"]);
            if(mouseYPtr == IntPtr.Zero) return null;

            unsafe
            {
                int* mouseY = (int*)mouseYPtr.ToPointer();
                return *mouseY;
            }
        }
    }
}