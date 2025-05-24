// /frontend/js/files.js
const api = "https://localhost:5001/api";
const token = localStorage.getItem("token");

if (!token) {
    alert("You must login first");
    window.location.href = "index.html";
}

function uploadFile() {
    const file = document.getElementById("fileInput").files[0];
    const formData = new FormData();
    formData.append("file", file);

    fetch(`${api}/files`, {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` },
        body: formData
    }).then(() => {
        alert("Uploaded");
        listFiles();
    });
}

function listFiles() {
    fetch(`${api}/files`, {
        headers: { Authorization: `Bearer ${token}` }
    })
        .then(res => res.json())
        .then(data => {
            const fileList = document.getElementById("fileList");
            fileList.innerHTML = "";
            data.forEach(f => {
                const item = document.createElement("li");
                item.innerHTML = `
          <span>${f.name}</span>
          <button onclick="download('${f.name}')"
            class="ml-2 text-blue-600 underline">Download</button>
        `;
                fileList.appendChild(item);
            });
        });
}

function download(name) {
    window.location.href = `${api}/files/${name}?token=${token}`;
}

document.addEventListener("DOMContentLoaded", listFiles);
