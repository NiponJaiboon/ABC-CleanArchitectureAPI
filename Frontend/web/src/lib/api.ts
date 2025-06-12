const API_URL = process.env.NEXT_PUBLIC_API_URL;

type LoginResponse = {
    access_token: string;
    refresh_token?: string;
    expires_in?: number;
};

export async function getData() {
    const res = await fetch(`${API_URL}/api/products`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    });
    if (!res.ok) throw new Error('Fetch failed');
    return res.json();
}

export async function login(username: string, password: string): Promise<LoginResponse> {
    const res = await fetch(`${API_URL}/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }),
        credentials: "include", // << ตรงนี้สำคัญสำหรับ cookie
    });
    if (!res.ok) throw new Error("Login failed");
    const data: LoginResponse = await res.json();

    // ถ้าใช้ HttpOnly cookie ไม่ต้องเก็บ token ใน localStorage
    // localStorage.setItem("token", data.access_token);

    return data;
}