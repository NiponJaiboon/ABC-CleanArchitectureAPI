"use client";

import { useState } from "react";
import { login } from "@/lib/api"; // ปรับ path ให้ตรงกับโครงสร้างของคุณ

export default function LoginPage() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    try {
      const data = await login(username, password); // เรียกใช้ login จาก api.ts
      console.log("Login successful:", data);
      // ถ้าใช้ HttpOnly cookie ไม่ต้องเก็บ token ใน localStorage
      // localStorage.setItem("token", data.access_token);
      // TODO: redirect ไปหน้าอื่น
    } catch {
      setError("เข้าสู่ระบบไม่สำเร็จ");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen">
      <form onSubmit={handleLogin} className="flex flex-col gap-4 w-80">
        <h1 className="text-2xl font-bold mb-4">Login</h1>
        <input
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          className="border p-2 rounded"
          required
        />
        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="border p-2 rounded"
          required
        />
        <button
          type="submit"
          className="bg-blue-600 text-white py-2 rounded hover:bg-blue-700"
          disabled={loading}
        >
          {loading ? "กำลังเข้าสู่ระบบ..." : "เข้าสู่ระบบ"}
        </button>
        {error && <div className="text-red-500">{error}</div>}
      </form>
    </div>
  );
}
