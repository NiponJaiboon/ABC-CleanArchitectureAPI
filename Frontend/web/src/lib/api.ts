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
        credentials: "include", // สำคัญ! เพื่อให้ cookie ถูกส่งไป-กลับ
    });
    if (!res.ok) throw new Error("Login failed");
    const data: LoginResponse = await res.json();

    // ไม่ต้องเก็บ token ใน localStorage หรือ memory
    // Token จะถูกแนบอัตโนมัติใน cookie (HttpOnly) ทุกครั้งที่เรียก API

    return data;
}

export async function logout() {
    const res = await fetch(`${API_URL}/api/auth/logout`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include", // << ตรงนี้สำคัญสำหรับ cookie
    });
    if (!res.ok) throw new Error("Logout failed");
    return res.json();
}

export async function register(username: string, password: string): Promise<LoginResponse> {
    const res = await fetch(`${API_URL}/api/auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }),
        credentials: "include", // << ตรงนี้สำคัญสำหรับ cookie
    });
    if (!res.ok) throw new Error("Registration failed");
    const data: LoginResponse = await res.json();

    // ถ้าใช้ HttpOnly cookie ไม่ต้องเก็บ token ใน localStorage
    // localStorage.setItem("token", data.access_token);

    return data;
}

export async function refreshToken(): Promise<boolean> {
    const res = await fetch(`${API_URL}/api/auth/refresh`, { method: 'POST', credentials: 'include' });
    return res.ok;
}

export async function getUser() {
    const res = await fetch(`${API_URL}/api/auth/user`, {
        method: "GET",
        headers: { "Content-Type": "application/json" },
        credentials: "include", // << ตรงนี้สำคัญสำหรับ cookie
    });
    if (!res.ok) throw new Error("Fetch user failed");
    return res.json();
}

export async function getProducts() {
    const res = await fetch(`${API_URL}/api/products`, {
        method: "GET",
        headers: { "Content-Type": "application/json" },
        credentials: "include", // << ตรงนี้สำคัญสำหรับ cookie
    });
    if (!res.ok) throw new Error("Fetch products failed");
    return res.json();
}

export async function getProductById(id: string) {
    const res = await fetch(`${API_URL}/api/products/${id}`, {
        method: "GET",
        headers: { "Content-Type": "application/json" },
        credentials: "include", // << ตรงนี้สำคัญสำหรับ cookie
    });
    if (!res.ok) throw new Error("Fetch product failed");
    return res.json();
}

export async function getProfile() {
    const res = await fetch(`${API_URL}/api/Profile`, {
        method: "GET",
        headers: { "Content-Type": "application/json" },
        credentials: "include", // ใช้ cookie ในการยืนยันตัวตน
    });
    if (!res.ok) throw new Error("Fetch profile failed");
    return res.json();
}
