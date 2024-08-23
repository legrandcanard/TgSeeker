export function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

export async function withExecutionStateAsync(ref, fn, minTime) {
    ref.state = true;
    const startTime = new Date();
    const result = await fn();
    const endTime = new Date();
    const differenceMs = endTime - startTime;
    if (differenceMs < minTime) {
        await sleep(minTime - differenceMs);
        ref.state = false;
    }
    else
        ref.state = false;

    return result;
}