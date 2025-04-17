import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { CategoryType } from '@/types';
import { buttonVariants } from '@/components/variants/buttonVariants';
import { CategoryFilters } from '../CategoryFilters';

vi.mock('@/components/variants/buttonVariants', () => ({
  buttonVariants: vi.fn().mockImplementation(({ variant }) => {
    return `mock-button mock-variant-${variant || 'default'}`;
  })
}));

describe('CategoryFilters Component', () => {
  const mockCategories: CategoryType[] = ['MainCourse', 'Appetizers', 'Desserts'];
  const mockHandleCategoryToggle = vi.fn();
  
  const mockIsCategoryActive = (category: CategoryType) => category === 'Appetizers';

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders all category buttons', () => {
    render(
      <CategoryFilters
        categories={mockCategories}
        handleCategoryToggle={mockHandleCategoryToggle}
        isCategoryActive={mockIsCategoryActive}
      />
    );

    expect(screen.getByText('Main Courses')).toBeInTheDocument();
    expect(screen.getByText('Appetizers')).toBeInTheDocument();
    expect(screen.getByText('Desserts')).toBeInTheDocument();
  });

  it('applies correct variants to buttons based on active status', () => {
    render(
      <CategoryFilters
        categories={mockCategories}
        handleCategoryToggle={mockHandleCategoryToggle}
        isCategoryActive={mockIsCategoryActive}
      />
    );

    expect(buttonVariants).toHaveBeenCalledTimes(3);
    
    const calls = vi.mocked(buttonVariants).mock.calls;
    
    const mainCourseCall = calls.find(call => 
      call[0] && call[0].className && call[0].className.includes('min-w-[135px]'));
    const appetizerCall = calls.find(call => 
      call[0] && call[0].variant === 'primary');
    const dessertCall = calls.find(call => 
      call[0] && call[0].variant === 'secondary');
    
    expect(mainCourseCall?.[0].variant).toBe('secondary');
    expect(appetizerCall?.[0].variant).toBe('primary');
    expect(dessertCall?.[0].variant).toBe('secondary');
  });

  it('calls handleCategoryToggle with correct category when button is clicked', () => {
    render(
      <CategoryFilters
        categories={mockCategories}
        handleCategoryToggle={mockHandleCategoryToggle}
        isCategoryActive={mockIsCategoryActive}
      />
    );

    fireEvent.click(screen.getByText('Desserts'));
    
    expect(mockHandleCategoryToggle).toHaveBeenCalledWith('Desserts');
    
    fireEvent.click(screen.getByText('Main Courses'));
    
    expect(mockHandleCategoryToggle).toHaveBeenCalledWith('MainCourse');
  });

  it('applies size="sm" to all buttons', () => {
    render(
      <CategoryFilters
        categories={mockCategories}
        handleCategoryToggle={mockHandleCategoryToggle}
        isCategoryActive={mockIsCategoryActive}
      />
    );
    
    const calls = vi.mocked(buttonVariants).mock.calls;
    
    // Проверяем, что все кнопки имеют size="sm"
    calls.forEach(call => {
      expect(call[0]?.size).toBe('sm');
    });
  });
});