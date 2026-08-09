// Chart.js Interop for Blazor
let chartInstances = {};

window.createChart = function (canvasId, chartConfig) {
    console.log('createChart called for:', canvasId);

    // Destroy existing chart if it exists
    if (chartInstances[canvasId]) {
        console.log('Destroying existing chart:', canvasId);
        chartInstances[canvasId].destroy();
    }

    const ctx = document.getElementById(canvasId);
    if (ctx) {
        console.log('Canvas element found, creating chart');
        chartInstances[canvasId] = new Chart(ctx, chartConfig);
        console.log('Chart created successfully');
    } else {
        console.error('Canvas element not found:', canvasId);
    }
};

window.destroyChart = function (canvasId) {
    if (chartInstances[canvasId]) {
        chartInstances[canvasId].destroy();
        delete chartInstances[canvasId];
    }
};

// Dark Mode Toggle
window.toggleDarkMode = function () {
    const html = document.documentElement;
    const currentTheme = html.getAttribute('data-theme');
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';

    html.setAttribute('data-theme', newTheme);
    localStorage.setItem('theme', newTheme);

    return newTheme;
};

window.initializeTheme = function () {
    const savedTheme = localStorage.getItem('theme') || 'light';
    document.documentElement.setAttribute('data-theme', savedTheme);
    console.log('Theme initialized:', savedTheme);
    return savedTheme;
};

// Auto-initialize theme when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', function() {
        window.initializeTheme();
    });
} else {
    window.initializeTheme();
}

// Clean up chart instances on page navigation (using pagehide instead of unload)
window.addEventListener('pagehide', function() {
    Object.values(chartInstances).forEach(chart => {
        try {
            chart.destroy();
        } catch (e) {
            console.warn('Error destroying chart:', e);
        }
    });
    chartInstances = {};
});
