import { useCallback } from 'react';

export function useAutoRefreshFetch() {
  const fetchWithAutoRefresh = useCallback(async (url: string, options: RequestInit = {}) => {
    let response = await fetch(url, { ...options, credentials: 'include' });

    if (response.status === 401) {
      // เรียก refresh token endpoint
      const refreshRes = await fetch('/api/auth/refresh', { method: 'POST', credentials: 'include' });
      if (refreshRes.ok) {
        // ถ้า refresh สำเร็จ ลอง fetch ใหม่
        response = await fetch(url, { ...options, credentials: 'include' });
      } else {
        throw new Error('Session expired');
      }
    }

    return response;
  }, []);

  return fetchWithAutoRefresh;
}