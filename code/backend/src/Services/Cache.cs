using System.Collections.Concurrent;

namespace backend.src.Services;

public interface ICitizenCacheService
{
    void DisableCitizen(Ulid citizenId);
    void EnableCitizen(Ulid citizenId, Ulid circuitId);
    Ulid? GetCircuitsApprovedCitizen(Ulid circuitId);
    Ulid? GetCitizenCircuit(Ulid citizenId);
}

public class CitizenCacheService : ICitizenCacheService
{
    private readonly ConcurrentDictionary<Ulid, Ulid> _circuitToCitizenCache = new();
    private readonly ConcurrentDictionary<Ulid, Ulid> _citizenToCircuitCache = new();

    public void EnableCitizen(Ulid citizenId, Ulid circuitId)
    {
        if (_citizenToCircuitCache.TryGetValue(citizenId, out var oldCircuitId))
        {
            _circuitToCitizenCache.TryRemove(oldCircuitId, out _);
        }

        _citizenToCircuitCache[citizenId] = circuitId;
        _circuitToCitizenCache[circuitId] = citizenId;
    }

    public void DisableCitizen(Ulid citizenId)
    {
        // Atomically remove from the first cache while getting the value
        if (_citizenToCircuitCache.TryRemove(citizenId, out Ulid circuitId))
        {
            // Then remove the corresponding entry from the second cache
            _circuitToCitizenCache.TryRemove(circuitId, out _);
        }
    }

    public Ulid? GetCitizenCircuit(Ulid citizenId)
    {
        return _citizenToCircuitCache.TryGetValue(citizenId, out Ulid circuitId) ? circuitId : null;
    }

    public Ulid? GetCircuitsApprovedCitizen(Ulid circuitId)
    {
        return _circuitToCitizenCache.TryGetValue(circuitId, out Ulid citizenId) ? citizenId : null;
    }
}
