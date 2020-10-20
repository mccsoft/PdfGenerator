import React from 'react';
import { shallow } from 'enzyme';
import { InfinitePage } from '../InfinitePage/InfinitePage';
import { PageOrientation } from "../../../utils/PageOrientation";

describe('Infinite page component', () => {
    const child = (
        <div>Some children</div>
    );

    const sut = shallow(
        <InfinitePage pageOrientation={PageOrientation.Portrait}>
            {child}
        </InfinitePage>
    );

    test('should render children', () => {
        const wrappedChild = (
            <tr className={'infinity-page-content-tr'}>
                <td className={'infinity-page-content-td'}>
                    {child}
                </td>
            </tr>
        );

        expect(sut.contains(wrappedChild))
            .toBeTruthy();
    });
});
