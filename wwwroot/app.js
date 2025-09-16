const api = {
    base: '',
    auth: '/api/auth/login',
    me: '/api/auth/me',
    rooms: '/api/rooms',
    bookings: '/api/bookings',
    adminSummary: '/api/admin/summary',
    adminRooms: '/api/admin/rooms'
};

function saveToken(t) { localStorage.setItem('token', t); }
function getToken() { return localStorage.getItem('token'); }
function clearToken() { localStorage.removeItem('token'); }

function authHeaders() {
    const t = getToken();
    return t ? { 'Authorization': 'Bearer ' + t } : {};
}

async function apiGet(url) {
    const res = await fetch(url, { headers: { 'Accept': 'application/json', ...authHeaders() } });
    if (!res.ok) throw await safeJson(res);
    return res.json();
}

async function apiPost(url, body) {
    const res = await fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', ...authHeaders() },
        body: JSON.stringify(body)
    });
    if (!res.ok) throw await safeJson(res);
    return res.json();
}

async function apiPut(url, body) {
    const res = await fetch(url, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json', ...authHeaders() },
        body: JSON.stringify(body)
    });
    if (!res.ok) throw await safeJson(res);
    return res.json();
}

async function apiDelete(url) {
    const res = await fetch(url, { method: 'DELETE', headers: { ...authHeaders() } });
    if (!res.ok) throw await safeJson(res);
    return true;
}

async function safeJson(res) {
    try { return await res.json(); }
    catch { return { error: res.statusText || ('HTTP ' + res.status) }; }
}
