/*
    =============================================================================
    CS#Fixes
    Copyright (C) 2023-2024 Charles Barone <CharlesBarone> / hypnos <hyps.dev>
    =============================================================================

    This program is free software; you can redistribute it and/or modify it under
    the terms of the GNU General Public License, version 3.0, as published by the
    Free Software Foundation.

    This program is distributed in the hope that it will be useful, but WITHOUT
    ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
    FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
    details.

    You should have received a copy of the GNU General Public License along with
    this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using CSSharpFixes.Managers;
using Microsoft.Extensions.Logging;

namespace CSSharpFixes.Models;

public class Patch
{
    private string _patchName;
    private string _modulePath;
    private string _signatureName;
    private List<byte>? _originalBytes;
    private string _bytesHex;
    private List<byte> _bytes;
    private bool _isPatched;

    private readonly GameDataManager _gameDataManager;
    private readonly ILogger<CSSharpFixes> _logger;
    
    public Patch(string patchName, string modulePath, string signatureName, string bytesHex,
        GameDataManager gameDataManager, ILogger<CSSharpFixes> logger)
    {
        _patchName = patchName;
        _modulePath = modulePath;
        _signatureName = signatureName;
        _bytesHex = bytesHex;
        _bytes = Utils.HexToByte(_bytesHex);
        _gameDataManager = gameDataManager;
        _logger = logger;
        _isPatched = false;
    }

    ~Patch()
    {
        if(_isPatched)
        {
            UndoPatch();
        }
        
        _bytes.Clear();
    }
    
    public bool IsPatched() { return _isPatched; }
    public string GetPatchName() { return _patchName; }
    public string GetModulePath() { return _modulePath; }
    public string GetSignatureName() { return _signatureName; }
    public string GetBytesHex() { return _bytesHex; }
    public List<byte> GetBytes() { return _bytes; }
    public List<byte>? GetOriginalBytes() { return _originalBytes; }
    
    public void PerformPatch()
    {
        if(_isPatched) return;
        
        IntPtr address = _gameDataManager.GetAddress(_modulePath, _signatureName);
        if(address == IntPtr.Zero)
        {
            _logger.LogError(
                "[CSSharpFixes][Patch][PerformPatch()][Patch={patchName}][Signature={signatureName}] Error: Address not found.",
                _patchName, _signatureName);
            return;
        }
        
        // Backup original bytes at address with length of _bytes.Count
        _originalBytes = Utils.ReadBytesFromAddress(address, _bytes.Count);
       
        // Log the bytes in hex
        _logger.LogInformation("[CSSharpFixes][Patch][PerformPatch()][Patch={patchName}][Signature={signatureName}][OriginalBytes={originalBytes}]",
           _patchName, _signatureName, Utils.ByteToHex(_originalBytes));
       
        Utils.WriteBytesToAddress(address, _bytes);
        
        _isPatched = true;

        _logger.LogInformation(
            "[CSSharpFixes][Patch][PerformPatch()][Patch={patchName}][Signature={signatureName}][PatchedBytes={bytes}] Successful",
            _patchName, _signatureName, Utils.ByteToHex(_bytes));
    }

    public void UndoPatch()
    {
        if(!_isPatched) return;
        
        IntPtr address = _gameDataManager.GetAddress(_modulePath, _signatureName);
        if(address == IntPtr.Zero)
        {
            _logger.LogError(
                "[CSSharpFixes][Patch][UndoPatch()][Patch={patchName}][Signature={signatureName}] Error: Address not found.",
                _patchName, _signatureName);
            return;
        }
        
        if(_originalBytes == null)
        {
            _logger.LogError(
                "[CSSharpFixes][Patch][UndoPatch()][Patch={patchName}][Signature={signatureName}] Error: Original bytes are null.",
                _patchName, _signatureName);
            return;
        }
        
        Utils.WriteBytesToAddress(address, _originalBytes);
        
        _logger.LogInformation(
            "[CSSharpFixes][Patch][UndoPatch()][Patch={patchName}][Signature={signatureName}][OriginalBytes={originalBytes}] Successful",
            _patchName, _signatureName, Utils.ByteToHex(_originalBytes));
        
        _originalBytes.Clear();
        _originalBytes = null;
        
        _isPatched = false;
    }

}