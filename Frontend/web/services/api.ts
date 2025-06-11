export async function getData() {
    const res = await fetch('https://localhost:5001/api/products', {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    });
    if (!res.ok) throw new Error('Fetch failed');
    return res.json();
}