using System.Collections.Concurrent;
using backend.src.Models;
using CitizenData = (System.Ulid id, bool allowObserved);

namespace backend.src.Services;

public interface ICitizenCacheService
{
    void DisableCitizen(Ulid citizenId);
    void EnableCitizen(CitizenData citizenData, CircuitId circuitId);
    CitizenData? GetCircuitsApprovedCitizen(CircuitId circuitId);
    CircuitId? GetCitizenCircuit(Ulid citizenId);
}

public class CitizenCacheService : ICitizenCacheService
{
    private object Lock { get; init; } = new();
    private readonly ConcurrentDictionary<CircuitId, CitizenData> _circuitToCitizenCache = new();
    private readonly ConcurrentDictionary<Ulid, CircuitId> _citizenToCircuitCache = new();

    public void EnableCitizen(CitizenData citizenData, CircuitId circuitId)
    {
        lock (Lock)
        {
            if (_citizenToCircuitCache.TryGetValue(citizenData.id, out var oldCircuitId))
            {
                _circuitToCitizenCache.TryRemove(oldCircuitId, out _);
            }

            _citizenToCircuitCache[citizenData.id] = circuitId;
            _circuitToCitizenCache[circuitId] = citizenData;
        }
    }

    public void DisableCitizen(Ulid citizenId)
    {
        lock (Lock)
        {
            if (_citizenToCircuitCache.TryRemove(citizenId, out var circuitId))
            {
                _circuitToCitizenCache.TryRemove(circuitId, out _);
            }
        }
    }

    public CircuitId? GetCitizenCircuit(Ulid citizenId)
    {
        return _citizenToCircuitCache.TryGetValue(citizenId, out var circuitId) ? circuitId : null;
    }

    public CitizenData? GetCircuitsApprovedCitizen(CircuitId circuitId)
    {
        return _circuitToCitizenCache.TryGetValue(circuitId, out var citizenId) ? citizenId : null;
    }
}
