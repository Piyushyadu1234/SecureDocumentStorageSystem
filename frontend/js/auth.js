const api = "https://localhost:7075/api";
let token = localStorage.getItem("token");

const authMsg = document.getElementById("auth-msg");
const uploadMsg = document.getElementById("upload-msg");

function showSectionsAfterLogin() {
    document.getElementById("auth-section").classList.add("hidden");
    document.getElementById("upload-section").classList.remove("hidden");
    document.getElementById("documents-section").classList.remove("hidden");
    listDocuments();
}

async function login() {
    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value;

    const res = await fetch(`${api}/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password })
    });

    if (res.ok) {
        token = await res.text();
        localStorage.setItem("token", token);
        authMsg.textContent = "Login successful!";
        showSectionsAfterLogin();
    } else {
        const error = await res.text();
        authMsg.textContent = "Login failed: " + error;
    }
}

async function register() {
    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value;

    const res = await fetch(`${api}/auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password })
    });

    if (res.ok) {
        authMsg.textContent = "Registration successful! You can now login.";
    } else {
        const error = await res.text();
        authMsg.textContent = "Registration failed: " + error;
    }
}

async function uploadFile() {
    const fileInput = document.getElementById("file-input");
    if (!fileInput.files.length) {
        uploadMsg.textContent = "Please select a file";
        return;
    }

    const formData = new FormData();
    formData.append("file", fileInput.files[0]);

    const res = await fetch(`${api}/files/upload`, {
        method: "POST",
        headers: {
            "Authorization": `Bearer ${token}`
        },
        body: formData
    });

    if (res.ok) {
        const data = await res.json();
        uploadMsg.textContent = data.message || "File uploaded successfully";
        listDocuments();
    } else {
        uploadMsg.textContent = "Upload failed";
    }
}

async function listDocuments() {
    const res = await fetch(`${api}/files/list`, {
        headers: {
            "Authorization": `Bearer ${token}`
        }
    });

    if (!res.ok) {
        document.getElementById("documents-list").innerHTML = "Failed to load documents";
        return;
    }

    const files = await res.json();

    const ul = document.getElementById("documents-list");
    ul.innerHTML = "";

    files.forEach(file => {
        const li = document.createElement("li");
        li.textContent = file.name + " ";
        const btn = document.createElement("button");
        btn.textContent = "Download";
        btn.onclick = () => downloadFile(file.name);
        li.appendChild(btn);
        ul.appendChild(li);
    });
}

async function downloadFile(filename) {
    const res = await fetch(`${api}/files/${encodeURIComponent(filename)}`, {
        headers: { "Authorization": `Bearer ${token}` }
    });

    if (!res.ok) {
        alert("Download failed");
        return;
    }

    const blob = await res.blob();
    const url = URL.createObjectURL(blob);

    const a = document.createElement("a");
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    a.remove();

    URL.revokeObjectURL(url);
}

document.getElementById("login-btn").addEventListener("click", login);
document.getElementById("register-btn").addEventListener("click", register);
document.getElementById("upload-btn").addEventListener("click", uploadFile);

// Auto-login if token exists (optional)
if (token) {
    showSectionsAfterLogin();
}
