import { refreshToken } from '@/lib/api';
import { useState, useCallback } from 'react';

export function useAutoRefreshFetch() {
  const [refreshTokenStatus, setRefreshTokenStatus] = useState<string>("");

  const handleRefreshToken = useCallback(async () => {
    setRefreshTokenStatus("กำลังรีเฟรช...");
    try {
      await refreshToken(); // <--- การเรียกนี้ต้องใช้ POST
      setRefreshTokenStatus("รีเฟรชสำเร็จ!");
    } catch (err: unknown) {
      setRefreshTokenStatus(
        `รีเฟรชล้มเหลว: ${(err as Error).message || "Unknown error"}`
      );
    }
  }, []);

  return { handleRefreshToken, refreshTokenStatus };
}
