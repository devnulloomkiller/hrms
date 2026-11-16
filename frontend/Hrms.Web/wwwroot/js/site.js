const themeToggle = document.querySelector(".theme-toggle");
const themeToggleLabel = document.querySelector(".theme-toggle-label");
const pageProgress = document.getElementById("page-progress");

function updateThemeToggle() {
    if (!themeToggle || !themeToggleLabel) return;
    const isDark = document.documentElement.dataset.theme === "dark";
    themeToggleLabel.textContent = isDark ? "Светлая тема" : "Тёмная тема";
    themeToggle.setAttribute("aria-pressed", String(isDark));
    themeToggle.setAttribute("aria-label", isDark ? "Включить светлую тему" : "Включить тёмную тему");
}

if (themeToggle) {
    updateThemeToggle();
    themeToggle.addEventListener("click", () => {
        const nextTheme = document.documentElement.dataset.theme === "dark" ? "light" : "dark";
        document.documentElement.dataset.theme = nextTheme;
        try {
            localStorage.setItem("hrms-theme", nextTheme);
        } catch {
        }
        updateThemeToggle();
    });
}

function showPageProgress() {
    if (!pageProgress) return;
    pageProgress.setAttribute("aria-hidden", "false");
    pageProgress.classList.add("is-visible");
}

document.addEventListener("submit", event => {
    const form = event.target;
    if (!(form instanceof HTMLFormElement)) return;
    if (form.classList.contains("is-submitting")) {
        event.preventDefault();
        return;
    }
    if (!form.checkValidity()) return;
    form.classList.add("is-submitting");
    const submitButton = form.querySelector("button[type='submit']");
    if (submitButton) submitButton.setAttribute("aria-disabled", "true");
    showPageProgress();
});

document.addEventListener("click", event => {
    if (!(event.target instanceof Element)) return;
    const link = event.target.closest("a[href]");
    if (!link || link.target === "_blank" || link.getAttribute("href")?.startsWith("#")) return;
    showPageProgress();
});

const mobileMenu = document.getElementById("mobileMenu");
if (mobileMenu) {
    mobileMenu.addEventListener("hidden.bs.offcanvas", () => {
        document.querySelector("[data-bs-target='#mobileMenu']")?.focus();
    });
}
