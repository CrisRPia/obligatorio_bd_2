export function pickRandom<TItem>(arr: TItem[]): TItem | undefined {
    if (arr.length === 0) {
        return undefined;
    }

    return arr[Math.floor(Math.random() * arr.length)];
}

export function deepLog<T>(obj: T): T {
    console.dir(obj, { depth: null });

    return obj;
}
