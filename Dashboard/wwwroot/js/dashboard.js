// Dashboard JavaScript functions for Chart.js integration and interop
window.dashboardCharts = {
    charts: {},
    
    renderChart: function(canvasId, chartType, chartData) {
        // Destroy existing chart if it exists
        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
        }
        
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.warn('Canvas not found:', canvasId);
            return;
        }
        
        const ctx = canvas.getContext('2d');
        
        const options = {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: chartType === 'pie',
                    position: 'bottom'
                }
            }
        };
        
        // Add scales for bar and line charts
        if (chartType === 'bar' || chartType === 'line') {
            options.scales = {
                y: {
                    beginAtZero: true
                }
            };
        }
        
        this.charts[canvasId] = new Chart(ctx, {
            type: chartType,
            data: chartData,
            options: options
        });
    },
    
    destroyChart: function(canvasId) {
        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
            delete this.charts[canvasId];
        }
    }
};

// Dashboard drag and drop functionality
window.dashboardDragDrop = {
    init: function(gridSelector) {
        // Future: implement drag and drop
    }
};
