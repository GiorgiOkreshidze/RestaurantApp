import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { Menu } from '..';
import { useAppDispatch } from '@/app/hooks';
import { getAllDishes } from '@/app/thunks/dishesThunks';
import { useFilterState } from '@/hooks/useFiltersState';
import { useSelector } from 'react-redux';
import { selectDishes, selectDishesLoading } from '@/app/slices/dishesSlice';

vi.mock('@/components/shared', () => ({
  AllDishes: ({ dishes, loading }) => (
    <div data-testid="all-dishes">
      {loading ? 'Loading...' : `Dishes count: ${dishes.length}`}
    </div>
  ),
  PageBody: ({ children }) => <div data-testid="page-body">{children}</div>,
  PageHero: ({ children, variant }) => (
    <div data-testid="page-hero" data-variant={variant}>
      {children}
    </div>
  ),
  PreorderInfoBar: () => <div data-testid="preorder-info-bar">Preorder Info</div>,
}));

vi.mock('@/components/shared/CategoryFilters', () => ({
  CategoryFilters: ({ categories, handleCategoryToggle, isCategoryActive }) => (
    <div data-testid="category-filters">
      {categories.map((category) => (
        <button
          key={category}
          data-category={category}
          data-active={isCategoryActive(category)}
          onClick={() => handleCategoryToggle(category)}
        >
          {category}
        </button>
      ))}
    </div>
  ),
}));

vi.mock('@/components/shared/SortingOptions', () => ({
  SortingOptions: ({ options, value, onChange }) => (
    <select
      data-testid="sorting-options"
      value={value}
      onChange={(e) => onChange(e.target.value)}
    >
      {options.map((option) => (
        <option key={option.id} value={option.id}>
          {option.label}
        </option>
      ))}
    </select>
  ),
}));

vi.mock('@/components/ui', () => ({
  Text: ({ children, variant, tag, className }) => (
    <div data-testid="text" data-variant={variant} data-tag={tag} className={className}>
      {children}
    </div>
  ),
}));

vi.mock('@/hooks/useFiltersState', () => ({
  useFilterState: vi.fn(),
}));

vi.mock('@/app/hooks', () => ({
  useAppDispatch: vi.fn(),
}));

vi.mock('react-redux', async (importOriginal) => {
  const actual = await importOriginal();
  return {
    ...actual,
    useSelector: vi.fn(),
  };
});

vi.mock('@/app/slices/dishesSlice', () => ({
  selectDishes: vi.fn(),
  selectDishesLoading: vi.fn(),
}));

vi.mock('@/app/thunks/dishesThunks', () => ({
  getAllDishes: vi.fn(),
}));

describe('Menu Component', () => {
  const mockDishes = [
    { id: 1, name: 'Dish 1', price: 10 },
    { id: 2, name: 'Dish 2', price: 15 },
  ];
  
  const mockDispatch = vi.fn().mockImplementation((action) => action);
  const mockGetAllDishes = vi.fn().mockReturnValue({ type: 'test/action' });
  
  const mockFilterState = {
    filters: {
      activeCategory: [],
      sortBy: 'PopularityDesc',
    },
    handleCategoryToggle: vi.fn(),
    isCategoryActive: vi.fn().mockImplementation((category) => category === 'MainCourse'),
    setSort: vi.fn(),
  };

  beforeEach(() => {
    vi.clearAllMocks();
    
    // Настройка моков
    vi.mocked(useSelector).mockImplementation((selector) => {
      if (selector === selectDishes) return mockDishes;
      if (selector === selectDishesLoading) return false;
      return undefined;
    });
    
    vi.mocked(useAppDispatch).mockReturnValue(mockDispatch);
    vi.mocked(getAllDishes).mockImplementation(mockGetAllDishes);
    vi.mocked(useFilterState).mockReturnValue(mockFilterState);
  });

  it('renders all main components correctly', () => {
    render(<Menu />);
    
    // Проверяем, что все основные компоненты отрендерены
    expect(screen.getByTestId('preorder-info-bar')).toBeInTheDocument();
    expect(screen.getByTestId('page-hero')).toBeInTheDocument();
    expect(screen.getByTestId('page-body')).toBeInTheDocument();
    expect(screen.getByTestId('category-filters')).toBeInTheDocument();
    expect(screen.getByTestId('sorting-options')).toBeInTheDocument();
    expect(screen.getByTestId('all-dishes')).toBeInTheDocument();
    
    // Проверяем заголовки
    const textElements = screen.getAllByTestId('text');
    const menuTitle = textElements.find(el => el.textContent === 'Menu');
    expect(menuTitle).toBeInTheDocument();
    
    const restaurantTitle = textElements.find(el => el.textContent === 'Green & Tasty Restaurants');
    expect(restaurantTitle).toBeInTheDocument();
  });

  it('dispatches getAllDishes with correct parameters on initial render', () => {
    render(<Menu />);
    
    expect(mockGetAllDishes).toHaveBeenCalledWith({
      category: mockFilterState.filters.activeCategory,
      sortBy: mockFilterState.filters.sortBy,
    });
    expect(mockDispatch).toHaveBeenCalled();
  });

  it('passes correct category props to CategoryFilters', () => {
    render(<Menu />);
    
    const categoryFilters = screen.getByTestId('category-filters');
    const categoryButtons = categoryFilters.querySelectorAll('button');
    
    // Проверяем, что у нас 3 категории
    expect(categoryButtons.length).toBe(3);
    
    // Проверяем названия категорий
    expect(categoryButtons[0]).toHaveAttribute('data-category', 'Appetizers');
    expect(categoryButtons[1]).toHaveAttribute('data-category', 'MainCourse');
    expect(categoryButtons[2]).toHaveAttribute('data-category', 'Desserts');
    
    // Проверяем, что активные категории помечены правильно
    expect(categoryButtons[0]).toHaveAttribute('data-active', 'false');
    expect(categoryButtons[1]).toHaveAttribute('data-active', 'true');
    expect(categoryButtons[2]).toHaveAttribute('data-active', 'false');
  });

  it('dispatches getAllDishes when filter changes', async () => {
    render(<Menu />);
    
    // Очищаем первоначальный вызов при рендере
    mockDispatch.mockClear();
    mockGetAllDishes.mockClear();
    
    // Изменяем значение фильтров
    const newFilters = {
      ...mockFilterState.filters,
      activeCategory: ['Desserts'],
    };
    
    // Обновляем мок useFilterState
    vi.mocked(useFilterState).mockReturnValue({
      ...mockFilterState,
      filters: newFilters,
    });
    
    // Повторно рендерим компонент с новыми значениями
    render(<Menu />);
    
    // Проверяем, что getAllDishes был вызван с обновленными параметрами
    expect(mockGetAllDishes).toHaveBeenCalledWith({
      category: newFilters.activeCategory,
      sortBy: newFilters.sortBy,
    });
  });

  it('passes correct sort options to SortingOptions', () => {
    render(<Menu />);
    
    const sortingSelect = screen.getByTestId('sorting-options');
    const options = sortingSelect.querySelectorAll('option');
    
    // Проверяем количество опций сортировки
    expect(options.length).toBe(4);
    
    // Проверяем содержимое опций
    expect(options[0].textContent).toBe('Popularity Ascending');
    expect(options[1].textContent).toBe('Popularity Descending');
    expect(options[2].textContent).toBe('Price Ascending');
    expect(options[3].textContent).toBe('Price Descending');
    
    // Проверяем значение по умолчанию
    expect(sortingSelect).toHaveValue('PopularityDesc');
  });

  it('passes dishes and loading state to AllDishes', () => {
    // Сначала проверяем состояние без загрузки
    vi.mocked(useSelector).mockImplementation((selector) => {
      if (selector === selectDishes) return mockDishes;
      if (selector === selectDishesLoading) return false;
      return undefined;
    });
    
    const { unmount } = render(<Menu />);
    
    const allDishes = screen.getByTestId('all-dishes');
    expect(allDishes.textContent).toBe('Dishes count: 2');
    
    // Очищаем предыдущий рендер перед новым тестом
    unmount();
    
    // Теперь проверяем состояние загрузки
    vi.mocked(useSelector).mockImplementation((selector) => {
      if (selector === selectDishes) return mockDishes;
      if (selector === selectDishesLoading) return true;
      return undefined;
    });
    
    render(<Menu />);
    
    const loadingDishes = screen.getByTestId('all-dishes');
    expect(loadingDishes.textContent).toBe('Loading...');
  });
});