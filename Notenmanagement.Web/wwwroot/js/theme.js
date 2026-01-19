window.theme = {
    get: () => localStorage.getItem("theme") || "light",
    set: (value) => {
        const v = value === "dark" ? "dark" : "light";
        localStorage.setItem("theme", v);
        document.documentElement.setAttribute("data-bs-theme", v);
    },
    init: () => {
        const v = localStorage.getItem("theme") || "light";
        document.documentElement.setAttribute("data-bs-theme", v);
        return v;
    }
};